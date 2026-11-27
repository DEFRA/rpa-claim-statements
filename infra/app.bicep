targetScope = 'resourceGroup'

import { ContainerSize, FileShares, Tags } from 'modules/contracts.bicep'

@minLength(1)
param location string

@minLength(2)
@maxLength(6)
param prefix string

@minLength(2)
@maxLength(3)
param environmentName string

param tags Tags

@minLength(1)
@maxLength(128)
param databaseName string

@description('Repository names within the foundation ACR and immutable registry digests; build production and generator targets separately.')
@sealed()
param images {
  webRepository: string
  webDigest: string
  generatorRepository: string
  generatorDigest: string
}

@sealed()
param entra {
  tenantId: string
  clientId: string
  clientSecretName: string
}

param fileShares FileShares
param webSize ContainerSize
param generatorSize ContainerSize

@description('Must be explicitly approved: trust-all forwarded headers are safe only when ACA ingress is the sole trusted request path.')
@allowed([true])
param trustedIngressConfirmed bool

@description('Operational exclusion across Web, imports, revisions and job executions is mandatory; the platform cannot enforce it here.')
@allowed([true])
param nonOverlapControlsConfirmed bool

@allowed(['BPS', 'XB', 'ALL', 'I'])
param statementType string

@minValue(1)
@maxValue(2147483647)
param maximumBatchSize int

@minValue(1)
@maxValue(86400)
param jobTimeoutSeconds int

@minValue(1)
@maxValue(600)
param terminationGracePeriodSeconds int

@description('Empty host disables SMTP configuration, not the database tracking report. Username/password secret must both be empty or both supplied; credentials require TLS.')
@sealed()
param smtp {
  host: string
  @minValue(1)
  @maxValue(65535)
  port: int
  enableSsl: bool
  userName: string
  passwordSecretName: string
}

var stem = '${prefix}-${environmentName}-csg'
var globalStem = '${prefix}${environmentName}csg${take(uniqueString(resourceGroup().id, prefix, environmentName, 'csg'), 10)}'
var webHost = '${stem}-web.${environment.properties.defaultDomain}'
var webResources = split(webSize, ':')
var jobResources = split(generatorSize, ':')
var shares = [
  fileShares.templates
  fileShares.data
  fileShares.pdfArchive
  fileShares.pdfOutbound
]
var volumes = [
  for share in shares: {
    name: share.registrationName
    storageType: 'AzureFile'
    storageName: share.registrationName
    mountOptions: 'uid=1000,gid=1000,dir_mode=0770,file_mode=0660'
  }
]
var volumeMounts = [for share in shares: { volumeName: share.registrationName, mountPath: share.mountPath }]

resource environment 'Microsoft.App/managedEnvironments@2025-07-01' existing = {
  name: '${stem}-env'
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' existing = {
  name: '${stem}-id'
}

resource registry 'Microsoft.ContainerRegistry/registries@2025-04-01' existing = {
  name: 'acr${globalStem}'
}

resource vault 'Microsoft.KeyVault/vaults@2024-11-01' existing = {
  name: 'kv${globalStem}'
}

resource wrappingKey 'Microsoft.KeyVault/vaults/keys@2024-11-01' existing = {
  parent: vault
  name: 'data-protection'
}

resource keyAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(wrappingKey.id, identity.id, 'key-wrap')
  scope: wrappingKey
  properties: {
    principalId: identity.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'e147488a-f6f5-4113-8e2d-b22465e65bf6')
  }
}

resource sql 'Microsoft.Sql/servers@2025-01-01' existing = {
  name: 'sql${globalStem}'
}

resource keyRing 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
  name: 'st${globalStem}'
}

resource insights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: '${stem}-insights'
}

resource logs 'Microsoft.OperationalInsights/workspaces@2025-07-01' existing = {
  name: '${stem}-logs'
}

var smtpSecrets = empty(smtp.passwordSecretName) ? [] : [
  {
    name: 'smtp-password'
    keyVaultUrl: '${vault.properties.vaultUri}secrets/${smtp.passwordSecretName}'
    identity: identity.id
  }
]
var sharedEnvironment = concat([
  {
    name: 'ConnectionStrings__ClaimStatementsContext'
    value: 'Server=tcp:${sql.properties.fullyQualifiedDomainName},1433;Database=${databaseName};Authentication=Active Directory Managed Identity;User Id=${identity.properties.clientId};Encrypt=Mandatory;TrustServerCertificate=False;'
  }
  { name: 'CLAIMSTATEMENTS_ROOT', value: '/var/lib/claimstatements/work' }
  { name: 'CLAIMSTATEMENTS_APPLY_MIGRATIONS', value: 'false' }
  { name: 'APPLICATIONINSIGHTS_CONNECTION_STRING', value: insights.properties.ConnectionString }
  { name: 'Telemetry__ManagedIdentityClientId', value: identity.properties.clientId }
], empty(smtp.host) ? [] : [
  { name: 'Smtp__Host', value: smtp.host }
  { name: 'Smtp__Port', value: string(smtp.port) }
  { name: 'Smtp__EnableSsl', value: string(smtp.enableSsl) }
], empty(smtp.userName) ? [] : [
  { name: 'Smtp__UserName', value: smtp.userName }
  { name: 'Smtp__Password', secretRef: 'smtp-password' }
])

module web 'br/public:avm/res/app/container-app:0.23.0' = {
  dependsOn: [keyAccess]
  params: {
    name: '${stem}-web'
    location: location
    tags: tags
    enableTelemetry: false
    environmentResourceId: environment.id
    workloadProfileName: 'Consumption'
    managedIdentities: { userAssignedResourceIds: [identity.id] }
    activeRevisionsMode: 'Single'
    maxInactiveRevisions: 0
    ingressExternal: true
    ingressAllowInsecure: false
    ingressTargetPort: 8080
    ingressTransport: 'http'
    terminationGracePeriodSeconds: terminationGracePeriodSeconds
    registries: [{ server: registry.properties.loginServer, identity: identity.id }]
    secrets: concat(smtpSecrets, [
      {
        name: 'entra-client-secret'
        keyVaultUrl: '${vault.properties.vaultUri}secrets/${entra.clientSecretName}'
        identity: identity.id
      }
    ])
    volumes: volumes
    containers: [
      {
        name: 'web'
        image: '${registry.properties.loginServer}/${images.webRepository}@${images.webDigest}'
        resources: { cpu: json(webResources[0]), memory: webResources[1] }
        volumeMounts: volumeMounts
        env: concat(sharedEnvironment, [
          { name: 'ASPNETCORE_ENVIRONMENT', value: 'Production' }
          { name: 'ASPNETCORE_HTTP_PORTS', value: '8080' }
          { name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED', value: string(trustedIngressConfirmed) }
          { name: 'AllowedHosts', value: webHost }
          { name: 'AzureAd__Instance', value: az.environment().authentication.loginEndpoint }
          { name: 'AzureAd__TenantId', value: entra.tenantId }
          { name: 'AzureAd__ClientId', value: entra.clientId }
          { name: 'AzureAd__CallbackPath', value: '/signin-oidc' }
          { name: 'AzureAd__SignedOutCallbackPath', value: '/signout-callback-oidc' }
          { name: 'AzureAd__ClientCredentials__0__SourceType', value: 'ClientSecret' }
          { name: 'AzureAd__ClientCredentials__0__ClientSecret', secretRef: 'entra-client-secret' }
          { name: 'DataProtection__Enabled', value: 'true' }
          {
            name: 'DataProtection__BlobUri'
            value: 'https://${keyRing.name}.blob.${az.environment().suffixes.storage}/data-protection/keys.xml'
          }
          { name: 'DataProtection__KeyIdentifier', value: '${vault.properties.vaultUri}keys/data-protection' }
          { name: 'DataProtection__ApplicationName', value: stem }
          { name: 'DataProtection__ManagedIdentityClientId', value: identity.properties.clientId }
        ])
        probes: [
          for probeType in ['Startup', 'Liveness']: {
            type: probeType
            httpGet: {
              path: '/health/live'
              port: 8080
              scheme: 'HTTP'
              httpHeaders: [{ name: 'Host', value: webHost }]
            }
            initialDelaySeconds: 10
            periodSeconds: 10
            timeoutSeconds: 3
            failureThreshold: probeType == 'Startup' ? 30 : 3
          }
        ]
      }
    ]
    scaleSettings: { minReplicas: 1, maxReplicas: 1 }
    diagnosticSettings: [{ name: 'platform', workspaceResourceId: logs.id }]
  }
}

resource generator 'Microsoft.App/jobs@2025-07-01' = {
  name: '${stem}-generator'
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: { '${identity.id}': {} }
  }
  properties: {
    environmentId: environment.id
    workloadProfileName: 'Consumption'
    configuration: {
      triggerType: 'Manual'
      replicaTimeout: jobTimeoutSeconds
      replicaRetryLimit: 0
      manualTriggerConfig: { parallelism: 1, replicaCompletionCount: 1 }
      registries: [{ server: registry.properties.loginServer, identity: identity.id }]
      secrets: smtpSecrets
    }
    template: {
      volumes: volumes
      containers: [
        {
          name: 'generator'
          image: '${registry.properties.loginServer}/${images.generatorRepository}@${images.generatorDigest}'
          command: ['dotnet', 'RPA.ClaimStatements.Generator.dll']
          args: statementType == 'I' ? ['I'] : [statementType, string(maximumBatchSize)]
          resources: { cpu: json(jobResources[0]), memory: jobResources[1] }
          env: sharedEnvironment
          volumeMounts: volumeMounts
        }
      ]
    }
  }
}

output webOrigin string = 'https://${webHost}'
output signInCallback string = 'https://${webHost}/signin-oidc'
output signOutCallback string = 'https://${webHost}/signout-callback-oidc'
output generatorResourceId string = generator.id
output operationalExclusionAcknowledged bool = nonOverlapControlsConfirmed
