#requires -Version 7.0
[CmdletBinding()]
param(
    [string] $BicepPath = 'bicep',
    [string] $FoundationParameters,
    [string] $StorageParameters,
    [string] $ApplicationParameters
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Assert-Condition([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Copy-Value($Value) {
    ConvertFrom-Json -InputObject (ConvertTo-Json -InputObject $Value -Depth 100) -AsHashtable
}

function Test-Equivalent($Left, $Right) {
    if ($Left -is [hashtable] -and $Right -is [hashtable]) {
        if ($Left.Count -ne $Right.Count) { return $false }
        foreach ($key in $Left.Keys) {
            if (-not $Right.ContainsKey($key) -or -not (Test-Equivalent $Left[$key] $Right[$key])) { return $false }
        }
        return $true
    }
    return $Left -ceq $Right
}

function Test-Value($Value, [hashtable] $Schema, [hashtable] $Template, [string] $Context, [bool] $Incomplete) {
    if ($Schema.ContainsKey('$ref')) {
        $definition = $Schema['$ref'].Replace('#/definitions/', '')
        $Schema = $Template.definitions[$definition]
    }
    if ($null -eq $Value -and $Incomplete) { return }
    Assert-Condition ($null -ne $Value) "${Context}: value is required."
    if ($Schema.ContainsKey('discriminator')) {
        $discriminator = $Schema.discriminator
        $property = $discriminator.propertyName
        Assert-Condition ($Value -is [hashtable] -and $Value.ContainsKey($property)) "${Context}.${property}: discriminator is required."
        Assert-Condition ($Value[$property] -is [string] -and $discriminator.mapping.ContainsKey($Value[$property])) "${Context}.${property}: value is not allowed."
        Test-Value $Value $discriminator.mapping[$Value[$property]] $Template $Context $Incomplete
        return
    }
    $validType = switch ($Schema.type) {
        'string' { $Value -is [string] }
        'bool' { $Value -is [bool] }
        'int' { $Value -is [int] -or $Value -is [long] -or $Value -is [bigint] }
        'object' { $Value -is [hashtable] }
        'array' { $Value -is [array] }
        default { throw "${Context}: unsupported schema type." }
    }
    Assert-Condition $validType "${Context}: incorrect type."
    if ($Schema.ContainsKey('allowedValues')) {
        Assert-Condition ($Value -cin $Schema.allowedValues) "${Context}: value is not allowed."
    }
    foreach ($bound in @('minLength', 'maxLength', 'minValue', 'maxValue')) {
        if (-not $Schema.ContainsKey($bound)) { continue }
        $measure = if ($bound.EndsWith('Length')) { $Value.Length } else { $Value }
        $valid = if ($bound.StartsWith('min')) { $measure -ge $Schema[$bound] } else { $measure -le $Schema[$bound] }
        Assert-Condition $valid "${Context}: $bound constraint failed."
    }
    if ($Schema.type -eq 'object') {
        $properties = if ($Schema.ContainsKey('properties')) { $Schema.properties } else { @{} }
        foreach ($key in $properties.Keys) {
            $property = $properties[$key]
            if (-not $property.ContainsKey('nullable') -or -not $property.nullable) {
                Assert-Condition ($Value.ContainsKey($key)) "${Context}.${key}: property is required."
            }
        }
        foreach ($key in $Value.Keys) {
            if ($properties.ContainsKey($key)) {
                Test-Value $Value[$key] $properties[$key] $Template "$Context.$key" $Incomplete
            } elseif ($Schema.ContainsKey('additionalProperties') -and $Schema.additionalProperties -is [hashtable]) {
                Test-Value $Value[$key] $Schema.additionalProperties $Template "$Context.$key" $Incomplete
            } else {
                throw "${Context}.${key}: unknown property."
            }
        }
    }
}

function Test-ResourceId([string] $Value, [string] $ResourceType, [string] $Context) {
    $guid = '[0-9a-f]{8}-(?:[0-9a-f]{4}-){3}[0-9a-f]{12}'
    $suffix = if ($ResourceType -eq 'Microsoft.Network/virtualNetworks/subnets') {
        'Microsoft\.Network/virtualNetworks/[^/<>\s]+/subnets/[^/<>\s]+'
    } else {
        [regex]::Escape($ResourceType) + '/[^/<>\s]+'
    }
    Assert-Condition ($Value -imatch "^/subscriptions/$guid/resourceGroups/[^/<>\s]+/providers/$suffix$") "${Context}: invalid resource ID."
    Assert-Condition ($Value.Split('/')[2] -ne [guid]::Empty.ToString()) "${Context}: empty subscription ID."
}

function Test-Guid([string] $Value, [string] $Context) {
    $parsed = [guid]::Empty
    Assert-Condition ([guid]::TryParseExact($Value, 'D', [ref] $parsed) -and $parsed -ne [guid]::Empty) "${Context}: nonempty GUID required."
}

function Test-Monitoring([hashtable] $Values) {
    $scope = $Values.monitoringAmpls
    if ($scope.mode -ceq 'Existing') {
        Test-ResourceId $scope.resourceId 'Microsoft.Insights/privateLinkScopes' 'monitoringAmpls.resourceId'
        if ($scope.associationOwner -ceq 'Project') {
            foreach ($key in @('workspaceAssociationName', 'applicationInsightsAssociationName')) {
                Assert-Condition ($scope.ContainsKey($key) -and $scope[$key] -cmatch '^[A-Za-z0-9._-]{1,150}$') 'monitoringAmpls: existing Project scope requires valid association names.'
            }
            Assert-Condition ($scope.workspaceAssociationName -ine $scope.applicationInsightsAssociationName) 'monitoringAmpls: association names must be distinct.'
        } else {
            Assert-Condition (-not $scope.ContainsKey('workspaceAssociationName') -and -not $scope.ContainsKey('applicationInsightsAssociationName')) 'monitoringAmpls: Platform associations must omit project association names.'
        }
    }
    $endpoint = $Values.monitoringPrivateEndpoint
    if ($endpoint.mode -ceq 'Existing') {
        Test-ResourceId $endpoint.resourceId 'Microsoft.Network/privateEndpoints' 'monitoringPrivateEndpoint.resourceId'
        Assert-Condition ($scope.mode -ceq 'Existing') 'monitoringPrivateEndpoint: Existing endpoint requires Existing AMPLS.'
    }
    $zones = @{
        monitor = 'privatelink.monitor.azure.com'
        oms = 'privatelink.oms.opinsights.azure.com'
        ods = 'privatelink.ods.opinsights.azure.com'
        agentsvc = 'privatelink.agentsvc.azure-automation.net'
    }
    foreach ($key in $zones.Keys) {
        $dns = $Values.monitoringPrivateDns[$key]
        if ($dns.zone.mode -ceq 'Create') {
            Assert-Condition ($dns.link.mode -ceq 'Create') 'monitoringPrivateDns: new zone requires a new VNet link.'
            Assert-Condition ($endpoint.mode -ceq 'Create') 'monitoringPrivateDns: Existing endpoint requires Existing zones.'
        } else {
            Test-ResourceId $dns.zone.resourceId 'Microsoft.Network/privateDnsZones' "monitoringPrivateDns.$key.zone.resourceId"
            Assert-Condition ($dns.zone.resourceId.Split('/')[-1] -ieq $zones[$key]) 'monitoringPrivateDns: wrong Monitor service zone.'
        }
    }
    foreach ($key in @('monitor', 'oms', 'ods', 'agentsvc', 'blobLink')) {
        $link = if ($key -ceq 'blobLink') { $Values.monitoringPrivateDns.blobLink } else { $Values.monitoringPrivateDns[$key].link }
        if ($link.mode -cne 'External') {
            Assert-Condition ($link.name -cmatch '^[A-Za-z0-9._-]{1,80}$') 'monitoringPrivateDns: explicit valid VNet link name required.'
        }
    }
}

function Test-Shares([hashtable] $Shares) {
    Assert-Condition ($Shares.templates.mountPath -ceq '/var/lib/claimstatements/work/Templates') 'fileShares: templates must use the image template path.'
    $names = @()
    $paths = @()
    $targets = @()
    foreach ($key in @('templates', 'data', 'pdfArchive', 'pdfOutbound')) {
        $share = $Shares[$key]
        Assert-Condition ($share.registrationName -cmatch '^[a-z][a-z0-9-]{0,58}[a-z0-9]$') 'fileShares: invalid registration name.'
        Assert-Condition ($share.accountName -cmatch '^[a-z0-9]{3,24}$') 'fileShares: invalid storage account name.'
        Assert-Condition ($share.shareName -cmatch '^[a-z0-9][a-z0-9-]{1,61}[a-z0-9]$' -and $share.shareName -notmatch '--') 'fileShares: invalid share name.'
        Assert-Condition ($share.accountKeySecretName -cmatch '^[A-Za-z0-9-]{1,127}$') 'fileShares: invalid account-key secret name.'
        $path = $share.mountPath
        Assert-Condition ($path -cmatch '^/var/lib/claimstatements/[A-Za-z0-9_-]+(?:/[A-Za-z0-9_-]+)*$') 'fileShares: canonical application mount path required.'
        if ($key -ne 'templates') {
            Assert-Condition ($path -cne '/var/lib/claimstatements/work' -and -not $path.StartsWith('/var/lib/claimstatements/work/')) 'fileShares: persistent data must not hide working folders.'
            Assert-Condition ($path -cne '/var/lib/claimstatements/home' -and -not $path.StartsWith('/var/lib/claimstatements/home/') -and
                $path -cne '/var/lib/claimstatements/scratch' -and -not $path.StartsWith('/var/lib/claimstatements/scratch/')) 'fileShares: persistent data must not hide runtime scratch folders.'
        }
        Assert-Condition ($share.registrationName -cnotin $names) 'fileShares: duplicate registration name.'
        $target = "$($share.accountName)/$($share.shareName)"
        Assert-Condition ($target -cnotin $targets) 'fileShares: each purpose needs a distinct share.'
        foreach ($previous in $paths) {
            Assert-Condition ($path -cne $previous -and -not $path.StartsWith("$previous/") -and -not $previous.StartsWith("$path/")) 'fileShares: mount paths overlap.'
        }
        $names += $share.registrationName
        $paths += $path
        $targets += $target
    }
}

function Test-Stage([string] $Stage, [hashtable] $Values, [hashtable] $Template) {
    foreach ($key in $Template.parameters.Keys) {
        Assert-Condition ($Values.ContainsKey($key)) "${Stage}.${key}: parameter is required."
        Test-Value $Values[$key] $Template.parameters[$key] $Template "$Stage.$key" $false
    }
    foreach ($key in $Values.Keys) {
        Assert-Condition ($Template.parameters.ContainsKey($key)) "${Stage}.${key}: unknown parameter."
    }
    Assert-Condition ($Values.prefix -cmatch '^[a-z0-9]{2,6}$') 'prefix: use 2-6 lowercase alphanumeric characters.'
    Assert-Condition ($Values.environmentName -cmatch '^[a-z0-9]{2,3}$') 'environmentName: use 2-3 lowercase alphanumeric characters.'
    if ($Stage -ne 'storage') {
        Assert-Condition ($Values.location -cmatch '^[a-z0-9]+$') 'location: explicit Azure region name required.'
        Assert-Condition ($Values.tags.environment -ceq $Values.environmentName) 'tags: environment must match environmentName.'
        foreach ($value in $Values.tags.Values) {
            Assert-Condition (-not [string]::IsNullOrWhiteSpace($value)) 'tags: blank values are not allowed.'
        }
    }
    if ($Stage -eq 'main') {
        Assert-Condition ($Values.sqlLocation -cmatch '^[a-z0-9]+$') 'sqlLocation: explicit Azure region name required.'
        foreach ($value in $Values.acaEnvironmentTags.Values) {
            Assert-Condition (-not [string]::IsNullOrWhiteSpace($value)) 'acaEnvironmentTags: blank values are not allowed.'
        }
        Test-ResourceId $Values.acaSubnetResourceId 'Microsoft.Network/virtualNetworks/subnets' 'acaSubnetResourceId'
        Test-ResourceId $Values.privateEndpointSubnetResourceId 'Microsoft.Network/virtualNetworks/subnets' 'privateEndpointSubnetResourceId'
        Assert-Condition ($Values.acaSubnetResourceId -ine $Values.privateEndpointSubnetResourceId) 'network: subnets must be distinct.'
        Assert-Condition (($Values.acaSubnetResourceId -ireplace '/subnets/[^/]+$', '') -ieq ($Values.privateEndpointSubnetResourceId -ireplace '/subnets/[^/]+$', '')) 'network: subnets must belong to the same approved VNet.'
        $zones = @{
            sql = 'privatelink.database.windows.net'
            vault = 'privatelink.vaultcore.azure.net'
            blob = 'privatelink.blob.core.windows.net'
            registry = 'privatelink.azurecr.io'
        }
        foreach ($key in $zones.Keys) {
            $id = $Values.privateDnsZoneResourceIds[$key]
            Test-ResourceId $id 'Microsoft.Network/privateDnsZones' "privateDnsZoneResourceIds.$key"
            Assert-Condition ($id.Split('/')[-1] -ceq $zones[$key]) 'privateDnsZoneResourceIds: wrong service zone.'
        }
        foreach ($link in $Values.serviceDnsLinks.Values) {
            if ($link.mode -cne 'External') {
                Assert-Condition ($link.name -cmatch '^[A-Za-z0-9._-]{1,80}$') 'serviceDnsLinks: explicit valid VNet link name required.'
            }
        }
        Test-Monitoring $Values
        Test-Guid $Values.database.administratorObjectId 'database.administratorObjectId'
        Assert-Condition (-not [string]::IsNullOrWhiteSpace($Values.database.administratorName)) 'database: administrator name must not be blank.'
        Assert-Condition ($Values.database.name -cmatch '^[A-Za-z0-9][A-Za-z0-9_.-]{0,127}$') 'database: invalid database name.'
        Assert-Condition ([decimal] $Values.database.minVcores -le $Values.database.maxVcores) 'database: minimum vCores exceed maximum.'
        foreach ($key in @('weeklyRetention', 'monthlyRetention', 'yearlyRetention')) {
            Assert-Condition ($Values.database.longTermRetention[$key] -cmatch '^P\d+[DWMY]$') 'database: invalid LTR duration.'
        }
    } elseif ($Stage -eq 'storage') {
        Test-Shares $Values.fileShares
    } else {
        Assert-Condition ($Values.databaseName -cmatch '^[A-Za-z0-9][A-Za-z0-9_.-]{0,127}$') 'databaseName: invalid database name.'
        Test-Guid $Values.entra.tenantId 'entra.tenantId'
        Test-Guid $Values.entra.clientId 'entra.clientId'
        Assert-Condition ($Values.entra.clientSecretName -cmatch '^[A-Za-z0-9-]{1,127}$') 'entra: invalid secret name.'
        foreach ($kind in @('web', 'generator')) {
            Assert-Condition ($Values.images["${kind}Repository"] -cmatch '^[a-z0-9]+(?:(?:[._]|__|-+)[a-z0-9]+)*(?:/[a-z0-9]+(?:(?:[._]|__|-+)[a-z0-9]+)*)*$') 'images: repository must not contain a host, tag or digest.'
            Assert-Condition ($Values.images["${kind}Digest"] -cmatch '^sha256:[0-9a-f]{64}$') 'images: immutable registry digest required.'
        }
        Test-Shares $Values.fileShares
        $smtp = $Values.smtp
        Assert-Condition ([string]::IsNullOrEmpty($smtp.userName) -eq [string]::IsNullOrEmpty($smtp.passwordSecretName)) 'smtp: username and password secret must be supplied together.'
        if (-not [string]::IsNullOrEmpty($smtp.userName)) {
            Assert-Condition ($smtp.enableSsl -and -not [string]::IsNullOrWhiteSpace($smtp.host)) 'smtp: credentials require a host and TLS.'
            Assert-Condition ($smtp.passwordSecretName -cmatch '^[A-Za-z0-9-]{1,127}$') 'smtp: invalid password secret name.'
        }
        Assert-Condition ([string]::IsNullOrEmpty($smtp.host) -or $smtp.host -cmatch '^[A-Za-z0-9][A-Za-z0-9.-]*[A-Za-z0-9]$') 'smtp: invalid relay host.'
    }
}

function Read-Parameters([string] $Path, [string] $Stage, [hashtable] $Template, [bool] $Incomplete) {
    $document = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json -AsHashtable
    Assert-Condition ($document -is [hashtable] -and $document.ContainsKey('parameters')) "${Stage}: parameter document required."
    $values = @{}
    foreach ($key in $document.parameters.Keys) {
        Assert-Condition ($Template.parameters.ContainsKey($key)) "${Stage}.${key}: unknown parameter."
        $entry = $document.parameters[$key]
        Assert-Condition ($entry -is [hashtable] -and $entry.Count -eq 1 -and $entry.ContainsKey('value')) "${Stage}.${key}: use value only; secret values do not belong in these files."
        Test-Value $entry.value $Template.parameters[$key] $Template "$Stage.$key" $Incomplete
        $values[$key] = $entry.value
    }
    foreach ($key in $Template.parameters.Keys) {
        Assert-Condition ($values.ContainsKey($key)) "${Stage}.${key}: parameter is required."
    }
    if (-not $Incomplete) { Test-Stage $Stage $values $Template }
    return $values
}

function Test-Consistency([hashtable] $Stages) {
    $names = @($Stages.Keys)
    foreach ($key in @('prefix', 'environmentName', 'location', 'tags', 'fileShares')) {
        $present = @($names | Where-Object { $Stages[$_].ContainsKey($key) })
        if ($present.Count -lt 2) { continue }
        $reference = $Stages[$present[0]][$key]
        foreach ($name in $present[1..($present.Count - 1)]) {
            Assert-Condition (Test-Equivalent $reference $Stages[$name][$key]) "stages: $key must match across supplied files."
        }
    }
    if ($Stages.ContainsKey('main') -and $Stages.ContainsKey('app')) {
        Assert-Condition ($Stages.main.database.name -ceq $Stages.app.databaseName) 'stages: database name must match.'
    }
}

function Expect-Failure([scriptblock] $Action, [string] $Expected) {
    $observed = $null
    try { & $Action | Out-Null } catch { $observed = $_.Exception.Message }
    Assert-Condition ($observed -ceq $Expected) "Negative test failed. Expected: $Expected; observed: $observed"
}

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "claim-statements-iac-$([guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Path $temporaryDirectory | Out-Null
try {
    $templates = @{}
    foreach ($file in Get-ChildItem -LiteralPath $PSScriptRoot -Recurse -Filter '*.bicep') {
        $output = Join-Path $temporaryDirectory "$($file.BaseName)-$([guid]::NewGuid().ToString('N')).json"
        $diagnostics = @(& $BicepPath build $file.FullName --outfile $output 2>&1)
        Assert-Condition ($LASTEXITCODE -eq 0 -and $diagnostics.Count -eq 0) "Bicep build failed or emitted diagnostics: $($file.Name)`n$($diagnostics -join [Environment]::NewLine)"
        $diagnostics = @(& $BicepPath lint $file.FullName --no-restore 2>&1)
        Assert-Condition ($LASTEXITCODE -eq 0 -and $diagnostics.Count -eq 0) "Bicep lint failed or emitted diagnostics: $($file.Name)`n$($diagnostics -join [Environment]::NewLine)"
        $key = [System.IO.Path]::GetRelativePath($PSScriptRoot, $file.FullName).Replace('.bicep', '')
        $templates[$key] = Get-Content -LiteralPath $output -Raw | ConvertFrom-Json -AsHashtable
    }
    Write-Host 'PASS: build and lint all Bicep files (zero diagnostics).'
    foreach ($stage in @('main', 'storage', 'app')) {
        Read-Parameters (Join-Path $PSScriptRoot "$stage.parameters.example.json") $stage $templates[$stage] $true | Out-Null
    }
    Write-Host 'PASS: example contracts match compiled schemas; null placeholders are intentionally incomplete.'

    $idPrefix = "/subscriptions/$([guid]::NewGuid())/resourceGroups/example-platform/providers"
    $shares = @{}
    foreach ($purpose in @('templates', 'data', 'pdfArchive', 'pdfOutbound')) {
        $name = $purpose.ToLowerInvariant()
        $shares[$purpose] = @{
            registrationName = $name; accountName = 'examplestorage'; shareName = $name
            accountKeySecretName = 'example-storage-key'; mountPath = "/var/lib/claimstatements/$name"
        }
    }
    $shares.templates.mountPath = '/var/lib/claimstatements/work/Templates'
    $tags = @{ application = 'example'; environment = 'dev'; owner = 'example'; costCentre = 'example' }
    $acaEnvironmentTags = @{
        'Created By' = 'example'; 'Created On' = 'example'; Customer = 'example'; 'email-id' = 'example@example.invalid'
        Offering = 'example'; Purpose = 'example'; Region = 'example'; Resourcegroupneedtagname = 'example'; tower = 'example'
    }
    $foundation = @{
        location = 'uksouth'; sqlLocation = 'uksouth'; prefix = 'ex'; environmentName = 'dev'; tags = $tags
        acaSubnetResourceId = "$idPrefix/Microsoft.Network/virtualNetworks/example/subnets/aca"
        privateEndpointSubnetResourceId = "$idPrefix/Microsoft.Network/virtualNetworks/example/subnets/endpoints"
        privateDnsZoneResourceIds = @{
            sql = "$idPrefix/Microsoft.Network/privateDnsZones/privatelink.database.windows.net"
            vault = "$idPrefix/Microsoft.Network/privateDnsZones/privatelink.vaultcore.azure.net"
            blob = "$idPrefix/Microsoft.Network/privateDnsZones/privatelink.blob.core.windows.net"
            registry = "$idPrefix/Microsoft.Network/privateDnsZones/privatelink.azurecr.io"
        }
        serviceDnsLinks = @{
            sql = @{ mode = 'Create'; name = 'example-network' }
            vault = @{ mode = 'Existing'; name = 'example-network' }
            registry = @{ mode = 'External' }
        }
        monitoring = @{
            logRetentionDays = 30; applicationInsightsRetentionDays = 30
        }
        monitoringAmpls = @{ mode = 'Create'; associationOwner = 'Project' }
        monitoringPrivateEndpoint = @{ mode = 'Create' }
        monitoringPrivateDns = @{ blobLink = @{ mode = 'Existing'; name = 'example-network' } }
        zoneRedundant = $false; acaEnvironmentTags = $acaEnvironmentTags; keyRingStorageSku = 'Standard_LRS'; blobSoftDeleteDays = 7; vaultSoftDeleteDays = 90
        database = @{
            name = 'Example'; administratorName = 'example-admin'; administratorObjectId = [guid]::NewGuid().ToString()
            administratorPrincipalType = 'Group'; maxVcores = 2; minVcores = '0.5'; maxSizeGiB = 32
            backupRedundancy = 'Local'; pointInTimeRetentionDays = 7
            longTermRetention = @{ weeklyRetention = 'P0W'; monthlyRetention = 'P0M'; yearlyRetention = 'P0Y'; weekOfYear = 1 }
        }
    }
    foreach ($key in @('monitor', 'oms', 'ods', 'agentsvc')) {
        $foundation.monitoringPrivateDns[$key] = @{
            zone = @{ mode = 'Create' }; link = @{ mode = 'Create'; name = 'example-network' }; zoneGroupOwner = 'Project'
        }
    }
    $storage = @{ prefix = 'ex'; environmentName = 'dev'; fileShares = $shares }
    $application = @{
        location = 'uksouth'; prefix = 'ex'; environmentName = 'dev'; tags = $tags; databaseName = 'Example'
        images = @{
            webRepository = 'example-web'; webDigest = 'sha256:' + ('a' * 64)
            generatorRepository = 'example-generator'; generatorDigest = 'sha256:' + ('b' * 64)
        }
        entra = @{ tenantId = [guid]::NewGuid().ToString(); clientId = [guid]::NewGuid().ToString(); clientSecretName = 'example-client' }
        fileShares = $shares; webSize = '1:2Gi'; generatorSize = '2:4Gi'
        trustedIngressConfirmed = $true; nonOverlapControlsConfirmed = $true
        statementType = 'ALL'; maximumBatchSize = 1; jobTimeoutSeconds = 600; terminationGracePeriodSeconds = 60
        smtp = @{ host = ''; port = 25; enableSsl = $false; userName = ''; passwordSecretName = '' }
    }
    $fixtures = @{ main = $foundation; storage = $storage; app = $application }
    foreach ($stage in $fixtures.Keys) { Test-Stage $stage $fixtures[$stage] $templates[$stage] }
    Test-Consistency $fixtures
    $roundTrip = @{}
    foreach ($stage in $fixtures.Keys) {
        $parameters = @{}
        foreach ($key in $fixtures[$stage].Keys) { $parameters[$key] = @{ value = $fixtures[$stage][$key] } }
        $path = Join-Path $temporaryDirectory "$stage.parameters.json"
        @{ parameters = $parameters } | ConvertTo-Json -Depth 50 | Set-Content -LiteralPath $path
        $roundTrip[$stage] = Read-Parameters $path $stage $templates[$stage] $false
    }
    Test-Consistency $roundTrip
    $mixedCase = Copy-Value $foundation
    $mixedCase.acaSubnetResourceId = $mixedCase.acaSubnetResourceId.Replace('/subnets/', '/SUBNETS/')
    Test-Stage 'main' $mixedCase $templates.main
    $existingMonitor = Copy-Value $foundation
    $existingMonitor.monitoringAmpls = @{
        mode = 'Existing'; resourceId = "$idPrefix/Microsoft.Insights/privateLinkScopes/example"
        associationOwner = 'Project'; workspaceAssociationName = 'example-logs'; applicationInsightsAssociationName = 'example-insights'
    }
    $monitorZoneNames = @{
        monitor = 'privatelink.monitor.azure.com'; oms = 'privatelink.oms.opinsights.azure.com'
        ods = 'privatelink.ods.opinsights.azure.com'; agentsvc = 'privatelink.agentsvc.azure-automation.net'
    }
    foreach ($key in $monitorZoneNames.Keys) {
        $existingMonitor.monitoringPrivateDns[$key].zone = @{
            mode = 'Existing'; resourceId = "$idPrefix/Microsoft.Network/privateDnsZones/$($monitorZoneNames[$key])"
        }
    }
    Test-Stage 'main' $existingMonitor $templates.main
    $platformMonitor = Copy-Value $existingMonitor
    $platformMonitor.monitoringAmpls = @{
        mode = 'Existing'; resourceId = "$idPrefix/Microsoft.Insights/privateLinkScopes/example"; associationOwner = 'Platform'
    }
    $platformMonitor.monitoringPrivateEndpoint = @{ mode = 'Existing'; resourceId = "$idPrefix/Microsoft.Network/privateEndpoints/example" }
    foreach ($key in $monitorZoneNames.Keys) {
        $platformMonitor.monitoringPrivateDns[$key].link = @{ mode = 'Existing'; name = 'example-network' }
    }
    Test-Stage 'main' $platformMonitor $templates.main
    $externalMonitor = Copy-Value $platformMonitor
    foreach ($key in $monitorZoneNames.Keys) { $externalMonitor.monitoringPrivateDns[$key].link = @{ mode = 'External' } }
    $externalMonitor.monitoringPrivateDns.blobLink = @{ mode = 'External' }
    Test-Stage 'main' $externalMonitor $templates.main

    $negativeCases = @(
        @{ stage = 'main'; change = { param($v) $v.sqlLocation = 'East US' }; error = 'sqlLocation: explicit Azure region name required.' },
        @{ stage = 'main'; change = { param($v) $v.acaEnvironmentTags.Customer = ' ' }; error = 'acaEnvironmentTags: blank values are not allowed.' },
        @{ stage = 'main'; change = { param($v) $v.acaEnvironmentTags.Remove('tower') }; error = "main.acaEnvironmentTags.tower: property is required." },
        @{ stage = 'main'; change = { param($v) $v.serviceDnsLinks.sql.name = ' ' }; error = 'serviceDnsLinks: explicit valid VNet link name required.' },
        @{ stage = 'main'; change = { param($v) $v.serviceDnsLinks.registry = @{ mode = 'Unknown' } }; error = 'main.serviceDnsLinks.registry.mode: value is not allowed.' },
        @{ stage = 'main'; change = { param($v) $v.serviceDnsLinks.vault.Remove('name') }; error = 'main.serviceDnsLinks.vault.name: property is required.' },
        @{ stage = 'main'; change = { param($v) $v.monitoringAmpls.mode = 'Unknown' }; error = 'main.monitoringAmpls.mode: value is not allowed.' },
        @{ stage = 'main'; change = { param($v) $v.monitoringAmpls.associationOwner = 'Platform' }; error = 'main.monitoringAmpls.associationOwner: value is not allowed.' },
        @{ stage = 'main'; change = { param($v) $v.monitoringPrivateEndpoint = @{ mode = 'Existing'; resourceId = 'endpoint' } }; error = 'monitoringPrivateEndpoint.resourceId: invalid resource ID.' },
        @{ stage = 'main'; change = { param($v) $v.monitoringPrivateEndpoint = @{ mode = 'Existing'; resourceId = "$idPrefix/Microsoft.Network/privateEndpoints/example" } }; error = 'monitoringPrivateEndpoint: Existing endpoint requires Existing AMPLS.' },
        @{ stage = 'main'; change = { param($v) $v.monitoringPrivateDns.monitor.link = @{ mode = 'External' } }; error = 'monitoringPrivateDns: new zone requires a new VNet link.' },
        @{ stage = 'main'; change = { param($v) $v.monitoringPrivateDns.monitor.link.name = ' ' }; error = 'monitoringPrivateDns: explicit valid VNet link name required.' },
        @{ stage = 'main'; change = { param($v) $v.monitoringPrivateDns.monitor.zoneGroupOwner = 'Platform' }; error = 'main.monitoringPrivateDns.monitor.zoneGroupOwner: value is not allowed.' },
        @{ stage = 'main'; change = { param($v) $v.monitoringPrivateDns.monitor.zone = @{ mode = 'Existing'; resourceId = $v.privateDnsZoneResourceIds.sql } }; error = 'monitoringPrivateDns: wrong Monitor service zone.' },
        @{ stage = 'main'; change = { param($v) $v.database.minVcores = '4' }; error = 'database: minimum vCores exceed maximum.' },
        @{ stage = 'main'; change = { param($v) $v.privateEndpointSubnetResourceId = $v.acaSubnetResourceId }; error = 'network: subnets must be distinct.' },
        @{ stage = 'main'; change = { param($v) $v.privateDnsZoneResourceIds.sql = $v.privateDnsZoneResourceIds.blob }; error = 'privateDnsZoneResourceIds: wrong service zone.' },
        @{ stage = 'main'; change = { param($v) $v.database.Remove('name') }; error = 'main.database.name: property is required.' },
        @{ stage = 'app'; change = { param($v) $v.trustedIngressConfirmed = $false }; error = 'app.trustedIngressConfirmed: value is not allowed.' },
        @{ stage = 'app'; change = { param($v) $v.nonOverlapControlsConfirmed = $false }; error = 'app.nonOverlapControlsConfirmed: value is not allowed.' },
        @{ stage = 'app'; change = { param($v) $v.images.webDigest = 'latest' }; error = 'images: immutable registry digest required.' },
        @{ stage = 'app'; change = { param($v) $v.entra.tenantId = 'common' }; error = 'entra.tenantId: nonempty GUID required.' },
        @{ stage = 'app'; change = { param($v) $v.smtp.userName = 'example'; $v.smtp.passwordSecretName = 'example'; $v.smtp.host = 'example.invalid' }; error = 'smtp: credentials require a host and TLS.' },
        @{ stage = 'app'; change = { param($v) $v.statementType = 'all' }; error = 'app.statementType: value is not allowed.' },
        @{ stage = 'app'; change = { param($v) $v.maximumBatchSize = 0 }; error = 'app.maximumBatchSize: minValue constraint failed.' },
        @{ stage = 'app'; change = { param($v) $v.extra = 'unused' }; error = 'app.extra: unknown parameter.' },
        @{ stage = 'app'; change = { param($v) $v.entra.extra = 'unused' }; error = 'app.entra.extra: unknown property.' },
        @{ stage = 'storage'; change = { param($v) $v.fileShares.data.mountPath = '/var/lib/claimstatements/work' }; error = 'fileShares: persistent data must not hide working folders.' },
        @{ stage = 'storage'; change = { param($v) $v.fileShares.pdfOutbound.mountPath = $v.fileShares.data.mountPath }; error = 'fileShares: mount paths overlap.' },
        @{ stage = 'storage'; change = { param($v) $v.fileShares.data.shareName = $v.fileShares.templates.shareName }; error = 'fileShares: each purpose needs a distinct share.' }
    )
    foreach ($case in $negativeCases) {
        $clone = Copy-Value $fixtures[$case.stage]
        & $case.change $clone | Out-Null
        Expect-Failure { Test-Stage $case.stage $clone $templates[$case.stage] } $case.error
    }
    $monitorNegativeCases = @(
        @{ change = { param($v) $v.monitoringAmpls.resourceId = 'scope' }; error = 'monitoringAmpls.resourceId: invalid resource ID.' },
        @{ change = { param($v) $v.monitoringAmpls.Remove('workspaceAssociationName') }; error = 'monitoringAmpls: existing Project scope requires valid association names.' },
        @{ change = { param($v) $v.monitoringAmpls.applicationInsightsAssociationName = $v.monitoringAmpls.workspaceAssociationName }; error = 'monitoringAmpls: association names must be distinct.' },
        @{ change = { param($v) $v.monitoringAmpls.associationOwner = 'Platform' }; error = 'monitoringAmpls: Platform associations must omit project association names.' },
        @{ change = { param($v) $v.monitoringPrivateEndpoint = @{ mode = 'Existing'; resourceId = "$idPrefix/Microsoft.Network/privateEndpoints/example" }; $v.monitoringPrivateDns.monitor.zone = @{ mode = 'Create' } }; error = 'monitoringPrivateDns: Existing endpoint requires Existing zones.' }
    )
    foreach ($case in $monitorNegativeCases) {
        $clone = Copy-Value $existingMonitor
        & $case.change $clone | Out-Null
        Expect-Failure { Test-Stage 'main' $clone $templates.main } $case.error
    }
    Expect-Failure { Read-Parameters (Join-Path $PSScriptRoot 'main.parameters.example.json') 'main' $templates.main $false } 'main.location: value is required.'
    $mismatch = Copy-Value $fixtures
    $mismatch.app.databaseName = 'Different'
    Expect-Failure { Test-Consistency $mismatch } 'stages: database name must match.'
    $mismatch = Copy-Value $fixtures
    $mismatch.app.fileShares.data.mountPath = '/var/lib/claimstatements/different'
    Expect-Failure { Test-Consistency $mismatch } 'stages: fileShares must match across supplied files.'
    foreach ($stage in $fixtures.Keys) { Test-Stage $stage $fixtures[$stage] $templates[$stage] }
    Write-Host "PASS: parameter-file round trips, mixed-case IDs, ownership modes and $($negativeCases.Count + $monitorNegativeCases.Count + 3) exact-error negative tests."

    $mainResources = @($templates.main.resources.Values)
    Assert-Condition (@($mainResources | Where-Object { $_.type -in @('Microsoft.Network/virtualNetworks', 'Microsoft.Network/virtualNetworks/subnets') }).Count -eq 0) 'Compiled ownership: no shared VNet or subnet writes allowed.'
    $scopeResources = @($templates['modules\monitor-private-link-scope'].resources)
    $scopeResource = @($scopeResources | Where-Object { $_.type -ceq 'Microsoft.Insights/privateLinkScopes' })[0]
    Assert-Condition ($scopeResource.condition -ceq "[parameters('createScope')]" -and
        $scopeResource.properties.accessModeSettings.ingestionAccessMode -ceq 'PrivateOnly' -and
        $scopeResource.properties.accessModeSettings.queryAccessMode -ceq 'PrivateOnly') 'Compiled Monitor: only explicit Create may write AMPLS; both modes must be PrivateOnly.'
    $associations = @($scopeResources | Where-Object { $_.type -ceq 'Microsoft.Insights/privateLinkScopes/scopedResources' })
    Assert-Condition ($associations.Count -eq 2 -and
        "[parameters('workspaceResourceId')]" -cin $associations.properties.linkedResourceId -and
        "[parameters('applicationInsightsResourceId')]" -cin $associations.properties.linkedResourceId) 'Compiled Monitor: workspace and Insights must both be associated.'
    Assert-Condition ($templates.main.resources.monitorScope.condition -ceq "[equals(parameters('monitoringAmpls').associationOwner, 'Project')]" -and
        $templates.main.resources.monitorScope.properties.parameters.createScope.value -ceq "[equals(parameters('monitoringAmpls').mode, 'Create')]") 'Compiled Monitor: existing scope and Platform associations must not be overwritten.'
    Assert-Condition ($templates.main.resources.monitorEndpoint.condition -ceq "[equals(parameters('monitoringPrivateEndpoint').mode, 'Create')]" -and
        'monitorScope' -cin $templates.main.resources.monitorEndpoint.dependsOn -and
        'monitorDnsConfiguration' -cin $templates.main.resources.monitorEndpoint.dependsOn) 'Compiled Monitor: endpoint creation must be explicit and follow associations/DNS.'
    $monitorEndpoint = $templates['modules\monitor-private-endpoint']
    $connection = $monitorEndpoint.resources.endpoint.properties.privateLinkServiceConnections
    Assert-Condition ($connection.Count -eq 1 -and $connection[0].properties.groupIds.Count -eq 1 -and
        $connection[0].properties.groupIds[0] -ceq 'azuremonitor' -and
        $connection[0].properties.privateLinkServiceId -ceq "[parameters('amplsResourceId')]") 'Compiled Monitor: endpoint must target AMPLS using azuremonitor.'
    Assert-Condition ($monitorEndpoint.parameters.dnsZoneResourceIds.minLength -eq 5 -and
        $monitorEndpoint.parameters.dnsZoneResourceIds.maxLength -eq 5 -and
        $monitorEndpoint.resources.dns.condition -ceq "[parameters('manageDnsZoneGroup')]" -and
        $monitorEndpoint.resources.dns.properties.copy[0].count -ceq "[length(parameters('dnsZoneResourceIds'))]") 'Compiled Monitor: explicit DNS group must include exactly five zones.'
    $monitorServices = $templates.main.variables.monitorServices
    Assert-Condition ($monitorServices.Count -eq 5 -and $monitorServices[4].name -ceq 'blob' -and
        $monitorServices[4].dns.zone.mode -ceq 'Existing' -and
        $monitorServices[4].dns.zone.resourceId -ceq "[parameters('privateDnsZoneResourceIds').blob]") 'Compiled Monitor: fifth zone must reuse key-ring Blob DNS without duplication.'
    foreach ($key in $monitorZoneNames.Keys) {
        $service = @($monitorServices | Where-Object { $_.name -ceq $key })
        Assert-Condition ($service.Count -eq 1 -and $service[0].zone -ceq $monitorZoneNames[$key]) 'Compiled Monitor: all four Monitor-specific zone names are required.'
    }
    $dnsResources = $templates['modules\private-dns-zone'].resources
    Assert-Condition ($dnsResources.zone.condition -ceq "[equals(parameters('configuration').zone.mode, 'Create')]" -and
        $dnsResources.link.condition -ceq "[equals(parameters('configuration').link.mode, 'Create')]" -and
        $dnsResources.link.properties.registrationEnabled -eq $false -and
        $dnsResources.link.properties.virtualNetwork.id -ceq "[parameters('vnetResourceId')]") 'Compiled Monitor: only requested zones/links may be written, without auto-registration.'
    foreach ($name in @('logs', 'insights')) {
        $arguments = $templates.main.resources[$name].properties.parameters
        Assert-Condition ($arguments.publicNetworkAccessForIngestion.value -ceq 'Disabled' -and
            $arguments.publicNetworkAccessForQuery.value -ceq 'Disabled') 'Compiled Monitor: public ingestion/query must remain disabled.'
    }
    Write-Host 'PASS: compiled AMPLS ownership, PrivateOnly modes, both associations, azuremonitor endpoint, five shared DNS zones and VNet-link ownership.'
    Assert-Condition (@($mainResources | Where-Object { $_.type -in @('Microsoft.App/containerApps', 'Microsoft.App/jobs', 'Microsoft.KeyVault/vaults/keys') }).Count -eq 0) 'Compiled staging: no direct workload or wrapping-key creation.'
    $vaultDeployment = $templates.main.resources.vault.properties
    Assert-Condition ($vaultDeployment.parameters.keys.value.Count -eq 0) 'Compiled staging: no AVM wrapping-key creation requested.'
    $environment = $templates.main.resources.environment.properties.parameters
    Assert-Condition ($environment.internal.value -eq $true -and $environment.publicNetworkAccess.value -ceq 'Disabled') 'Compiled privacy: ACA environment must be internal and private.'
    Assert-Condition ($environment.tags.value -ceq "[union(parameters('tags'), parameters('acaEnvironmentTags'))]") 'Compiled ACA tags: environment must union acaEnvironmentTags for managed-RG tag propagation.'
    Assert-Condition ($templates.main.resources.sql.properties.parameters.location.value -ceq "[parameters('sqlLocation')]") 'Compiled SQL region: server must use the separate sqlLocation parameter, not the shared location.'
    foreach ($name in @('vault', 'keyRing', 'registry', 'identity', 'logs', 'insights')) {
        Assert-Condition ($templates.main.resources[$name].properties.parameters.location.value -ceq "[parameters('location')]") "Compiled SQL region: only SQL should use sqlLocation; $name must remain on the shared location."
    }
    foreach ($name in @('vault', 'sql', 'keyRing', 'registry')) {
        $arguments = $templates.main.resources[$name].properties.parameters
        Assert-Condition ($arguments.publicNetworkAccess.value -ceq 'Disabled') "Compiled privacy: $name must be private."
    }
    $registry = $templates.main.resources.registry.properties.parameters
    Assert-Condition ($registry.networkRuleBypassOptions.value -ceq 'None' -and
        $registry.networkRuleBypassAllowedForTasks.value -eq $false -and $registry.networkRuleSetIpRules.value.Count -eq 0) 'Compiled privacy: registry must not retain trusted-service or IP bypass.'
    Assert-Condition ($templates.main.resources.sql.properties.parameters.firewallRules.value.Count -eq 0) 'Compiled privacy: no SQL public firewall exceptions.'
    foreach ($name in @('vault', 'keyRing')) {
        $acl = $templates.main.resources[$name].properties.parameters.networkAcls.value
        Assert-Condition ($acl.bypass -ceq 'None' -and $acl.defaultAction -ceq 'Deny') 'Compiled privacy: no Vault/Blob bypass.'
    }
    $endpointModules = @{
        sql = 'privateConnectionSql'; vault = 'privateConnectionVault'; blob = 'privateConnectionBlob'; registry = 'privateConnectionRegistry'
    }
    $serviceIndexes = @{ sql = 0; vault = 1; blob = 2; registry = 3 }
    $sourceModules = @{ sql = 'sql'; vault = 'vault'; blob = 'keyRing'; registry = 'registry' }
    foreach ($name in $serviceIndexes.Keys) {
        $endpoint = $templates.main.resources[$endpointModules[$name]]
        $arguments = $endpoint.properties.parameters
        $index = $serviceIndexes[$name]
        Assert-Condition ($arguments.groupId.value -ceq "[variables('privateServices')[$index].groupId]" -and
            $arguments.dnsZoneResourceId.value -ceq "[variables('privateServices')[$index].zoneId]" -and
            $arguments.targetResourceId.value -ceq "[variables('privateServices')[$index].id]") 'Compiled privacy: service endpoint group, target and DNS zone must reference the matching privateServices entry.'
        Assert-Condition ($sourceModules[$name] -cin $endpoint.dependsOn -and
            'serviceDnsConfiguration' -cin $endpoint.dependsOn -and 'monitorDnsConfiguration' -cin $endpoint.dependsOn) "Compiled privacy: $name endpoint must depend only on its own source resource plus DNS configuration."
        foreach ($other in $sourceModules.Keys) {
            if ($other -cne $name) {
                Assert-Condition ($sourceModules[$other] -cnotin $endpoint.dependsOn) "Compiled privacy: $name endpoint must not depend on unrelated service $other, or one service's failure would block the others."
            }
        }
    }
    Write-Host 'PASS: all service endpoint/DNS contracts, explicit DNS links and no public bypass.'
    $webArguments = $templates.app.resources.web.properties.parameters
    Assert-Condition ($webArguments.scaleSettings.value.minReplicas -eq 1 -and $webArguments.scaleSettings.value.maxReplicas -eq 1 -and
        $webArguments.activeRevisionsMode.value -ceq 'Single' -and $webArguments.ingressAllowInsecure.value -eq $false) 'Compiled web: singleton, single-revision, HTTPS-only configuration required.'
    $job = $templates.app.resources.generator
    Assert-Condition ($job.properties.configuration.triggerType -ceq 'Manual' -and $job.properties.configuration.replicaRetryLimit -eq 0 -and
        $job.properties.configuration.manualTriggerConfig.parallelism -eq 1) 'Compiled job: manual, no retries, one parallel replica required.'
    $storageText = ConvertTo-Json -InputObject $templates.storage -Depth 100
    Assert-Condition ($storageText.Contains('"accountKeyVaultProperties"') -and -not $storageText.Contains('"accountKey":')) 'Compiled storage: vault key references only.'
    $appText = ConvertTo-Json -InputObject $templates.app -Depth 100
    Assert-Condition ($appText.Contains('AzureAd__ClientCredentials__0__ClientSecret') -and -not $appText.Contains('/health/ready')) 'Compiled application: use actual identity settings and health endpoint.'
    foreach ($file in Get-ChildItem -LiteralPath $PSScriptRoot -Recurse -Filter '*.bicep') {
        $source = Get-Content -LiteralPath $file.FullName -Raw
        foreach ($match in [regex]::Matches($source, "'br/public:([^']+)'")) {
            Assert-Condition ($match.Groups[1].Value -cmatch '^avm/res/[a-z0-9/-]+:\d+\.\d+\.\d+$') 'AVM references must use explicit semantic version pins.'
        }
    }
    Write-Host 'PASS: compiled ownership, staging, privacy, job, storage-secret, application-contract and AVM-pin checks.'

    $supplied = @{}
    foreach ($entry in @(
        @{ stage = 'main'; path = $FoundationParameters },
        @{ stage = 'storage'; path = $StorageParameters },
        @{ stage = 'app'; path = $ApplicationParameters }
    )) {
        if (-not [string]::IsNullOrEmpty($entry.path)) {
            $supplied[$entry.stage] = Read-Parameters $entry.path $entry.stage $templates[$entry.stage] $false
        }
    }
    Test-Consistency $supplied
    Write-Host 'PASS: supplied parameter files, if any. No Azure calls, deployment or What-If performed.'
} finally {
    if (Test-Path -LiteralPath $temporaryDirectory) {
        Remove-Item -LiteralPath $temporaryDirectory -Recurse -Force
    }
}
