targetScope = 'resourceGroup'

import { Tags } from 'modules/contracts.bicep'
import { PrivateDns, Link } from 'modules/private-dns-zone.bicep'

@minLength(1)
param location string

@description('Explicit region for Microsoft.Sql/servers only, when SQL provisioning is restricted in the primary location for this subscription. Defaults to the primary location so existing deployments are unaffected.')
@minLength(1)
param sqlLocation string = location

@description('Owner-approved lowercase alphanumeric naming prefix, not a customer default.')
@minLength(2)
@maxLength(6)
param prefix string

@minLength(2)
@maxLength(3)
param environmentName string

param tags Tags

@description('Existing dedicated, delegated workload-profile subnet. This template never updates the VNet/subnet.')
param acaSubnetResourceId string

@description('Existing undelegated endpoint subnet with approved capacity and private endpoint policies.')
param privateEndpointSubnetResourceId string

@description('Existing platform-owned service zones. New endpoint zone groups manage records; monitoringPrivateDns.blobLink separately selects Blob VNet-link ownership.')
@sealed()
param privateDnsZoneResourceIds {
  sql: string
  vault: string
  blob: string
  registry: string
}

@description('Explicit CPH Create/Existing/External link ownership for existing service DNS zones. Blob link ownership remains in monitoringPrivateDns.blobLink.')
@sealed()
param serviceDnsLinks {
  sql: Link
  vault: Link
  registry: Link
}

@sealed()
param monitoring {
  @minValue(30)
  @maxValue(730)
  logRetentionDays: int
  applicationInsightsRetentionDays: 30 | 60 | 90 | 120 | 180 | 270 | 365 | 550 | 730
}

@sealed()
type NewAmpls = {
  mode: 'Create'
  associationOwner: 'Project'
}

@sealed()
type ExistingAmpls = {
  mode: 'Existing'
  resourceId: string
  associationOwner: 'Project' | 'Platform'
  workspaceAssociationName: string?
  applicationInsightsAssociationName: string?
}

@description('CPH ownership contract. Create only with approved DNS isolation; Existing scopes must already be PrivateOnly.')
@discriminator('mode')
param monitoringAmpls NewAmpls | ExistingAmpls

@sealed()
type NewMonitorEndpoint = {
  mode: 'Create'
}

@sealed()
type ExistingMonitorEndpoint = {
  mode: 'Existing'
  resourceId: string
}

@description('CPH endpoint ownership contract. Existing requires owner verification of target, approval, five-zone group and reachability.')
@discriminator('mode')
param monitoringPrivateEndpoint NewMonitorEndpoint | ExistingMonitorEndpoint

@description('CPH Monitor DNS ownership contracts. Blob reuses privateDnsZoneResourceIds.blob; only its link ownership is selected here.')
@sealed()
param monitoringPrivateDns {
  monitor: PrivateDns
  oms: PrivateDns
  ods: PrivateDns
  agentsvc: PrivateDns
  blobLink: Link
}

@description('Explicit additional tags applied only to the ACA Managed Environment, so Azure propagates them to its platform-managed infrastructure resource group (e.g. to satisfy a management-group tag policy on that managed RG). Not applied to other resources.')
@sealed()
param acaEnvironmentTags {
  'Created By': string
  'Created On': string
  Customer: string
  'email-id': string
  Offering: string
  Purpose: string
  Region: string
  Resourcegroupneedtagname: string
  tower: string
}

param zoneRedundant bool

@allowed(['Standard_LRS', 'Standard_ZRS', 'Standard_GRS', 'Standard_GZRS'])
param keyRingStorageSku string

@minValue(1)
@maxValue(365)
param blobSoftDeleteDays int

@minValue(7)
@maxValue(90)
param vaultSoftDeleteDays int

@description('Azure SQL Database only. External SQL Agent/BCP/xp_cmdshell workflows are NOT migrated by this template.')
@sealed()
param database {
  @minLength(1)
  @maxLength(128)
  name: string
  @minLength(1)
  administratorName: string
  administratorObjectId: string
  administratorPrincipalType: 'Group' | 'User'
  maxVcores: 2 | 4 | 6 | 8 | 10 | 12 | 14 | 16 | 18 | 20 | 24 | 32 | 40 | 64 | 80
  minVcores: '0.5' | '0.75' | '1' | '1.25' | '1.5' | '2' | '3' | '4' | '6' | '8' | '10' | '12' | '16' | '20' | '24' | '32' | '40'
  @minValue(1)
  @maxValue(4096)
  maxSizeGiB: int
  backupRedundancy: 'Local' | 'Zone' | 'Geo' | 'GeoZone'
  @minValue(1)
  @maxValue(35)
  pointInTimeRetentionDays: int
  longTermRetention: {
    weeklyRetention: string
    monthlyRetention: string
    yearlyRetention: string
    @minValue(1)
    @maxValue(52)
    weekOfYear: int
  }
}

var stem = '${prefix}-${environmentName}-csg'
var globalStem = '${prefix}${environmentName}csg${take(uniqueString(resourceGroup().id, prefix, environmentName, 'csg'), 10)}'
var virtualNetworkResourceId = join(take(split(acaSubnetResourceId, '/'), 9), '/')
var monitorAmplsId = monitoringAmpls.mode == 'Create'
  ? resourceId('Microsoft.Insights/privateLinkScopes', '${stem}-ampls')
  : monitoringAmpls.resourceId
var monitorEndpointId = monitoringPrivateEndpoint.mode == 'Create'
  ? resourceId('Microsoft.Network/privateEndpoints', '${stem}-monitor-pe')
  : monitoringPrivateEndpoint.resourceId
type MonitorDnsService = {
  name: string
  zone: string
  dns: PrivateDns
}

var monitorServices MonitorDnsService[] = [
  { name: 'monitor', zone: 'privatelink.monitor.azure.com', dns: monitoringPrivateDns.monitor }
  { name: 'oms', zone: 'privatelink.oms.opinsights.azure.com', dns: monitoringPrivateDns.oms }
  { name: 'ods', zone: 'privatelink.ods.opinsights.azure.com', dns: monitoringPrivateDns.ods }
  { name: 'agentsvc', zone: 'privatelink.agentsvc.azure-automation.net', dns: monitoringPrivateDns.agentsvc }
  {
    name: 'blob'
    zone: 'privatelink.blob.${az.environment().suffixes.storage}'
    dns: {
      zone: { mode: 'Existing', resourceId: privateDnsZoneResourceIds.blob }
      link: monitoringPrivateDns.blobLink
      zoneGroupOwner: 'Project'
    }
  }
]
var monitorZoneIds = [
  for service in monitorServices: service.dns.zone.mode == 'Create'
    ? resourceId('Microsoft.Network/privateDnsZones', service.zone)
    : service.dns.zone.resourceId
]

module identity 'br/public:avm/res/managed-identity/user-assigned-identity:0.6.0' = {
  params: {
    name: '${stem}-id'
    location: location
    tags: tags
    enableTelemetry: false
  }
}

module logs 'br/public:avm/res/operational-insights/workspace:0.16.1' = {
  params: {
    name: '${stem}-logs'
    location: location
    tags: tags
    enableTelemetry: false
    skuName: 'PerGB2018'
    dataRetention: monitoring.logRetentionDays
    features: { disableLocalAuth: true }
    publicNetworkAccessForIngestion: 'Disabled'
    publicNetworkAccessForQuery: 'Disabled'
  }
}

module insights 'br/public:avm/res/insights/component:0.8.0' = {
  params: {
    name: '${stem}-insights'
    location: location
    tags: tags
    enableTelemetry: false
    workspaceResourceId: logs.outputs.resourceId
    applicationType: 'web'
    kind: 'web'
    disableLocalAuth: true
    retentionInDays: monitoring.applicationInsightsRetentionDays
    publicNetworkAccessForIngestion: 'Disabled'
    publicNetworkAccessForQuery: 'Disabled'
    roleAssignments: [
      {
        principalId: identity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'Monitoring Metrics Publisher'
      }
    ]
  }
}

module registry 'br/public:avm/res/container-registry/registry:0.13.1' = {
  params: {
    name: 'acr${globalStem}'
    location: location
    tags: tags
    enableTelemetry: false
    acrSku: 'Premium'
    acrAdminUserEnabled: false
    anonymousPullEnabled: false
    roleAssignmentMode: 'LegacyRegistryPermissions'
    publicNetworkAccess: 'Disabled'
    networkRuleSetDefaultAction: 'Deny'
    networkRuleBypassOptions: 'None'
    networkRuleBypassAllowedForTasks: false
    networkRuleSetIpRules: []
    zoneRedundancy: zoneRedundant ? 'Enabled' : 'Disabled'
    diagnosticSettings: [{ name: 'platform', workspaceResourceId: logs.outputs.resourceId }]
    roleAssignments: [
      {
        principalId: identity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'AcrPull'
      }
    ]
  }
}

module vault 'br/public:avm/res/key-vault/vault:0.14.2' = {
  params: {
    name: 'kv${globalStem}'
    location: location
    tags: tags
    enableTelemetry: false
    sku: 'standard'
    enableRbacAuthorization: true
    enableSoftDelete: true
    enablePurgeProtection: true
    softDeleteRetentionInDays: vaultSoftDeleteDays
    enableVaultForDeployment: false
    enableVaultForTemplateDeployment: false
    enableVaultForDiskEncryption: false
    keys: []
    publicNetworkAccess: 'Disabled'
    networkAcls: { bypass: 'None', defaultAction: 'Deny' }
    diagnosticSettings: [{ name: 'platform', workspaceResourceId: logs.outputs.resourceId }]
    roleAssignments: [
      {
        principalId: identity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: 'Key Vault Secrets User'
      }
    ]
  }
}

module keyRing 'br/public:avm/res/storage/storage-account:0.33.1' = {
  params: {
    name: 'st${globalStem}'
    location: location
    tags: tags
    enableTelemetry: false
    skuName: keyRingStorageSku
    kind: 'StorageV2'
    allowSharedKeyAccess: false
    defaultToOAuthAuthentication: true
    allowBlobPublicAccess: false
    allowCrossTenantReplication: false
    publicNetworkAccess: 'Disabled'
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    networkAcls: { bypass: 'None', defaultAction: 'Deny' }
    fileServices: {}
    queueServices: {}
    tableServices: {}
    blobServices: {
      isVersioningEnabled: true
      deleteRetentionPolicyEnabled: true
      deleteRetentionPolicyDays: blobSoftDeleteDays
      containerDeleteRetentionPolicyEnabled: true
      containerDeleteRetentionPolicyDays: blobSoftDeleteDays
      containers: [
        {
          name: 'data-protection'
          publicAccess: 'None'
          roleAssignments: [
            {
              principalId: identity.outputs.principalId
              principalType: 'ServicePrincipal'
              roleDefinitionIdOrName: 'Storage Blob Data Contributor'
            }
          ]
        }
      ]
      diagnosticSettings: [{ name: 'platform', workspaceResourceId: logs.outputs.resourceId }]
    }
  }
}

module sql 'br/public:avm/res/sql/server:0.22.1' = {
  params: {
    name: 'sql${globalStem}'
    location: sqlLocation
    tags: tags
    enableTelemetry: false
    publicNetworkAccess: 'Disabled'
    minimalTlsVersion: '1.2'
    connectionPolicy: 'Proxy'
    firewallRules: []
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      principalType: database.administratorPrincipalType
      login: database.administratorName
      sid: database.administratorObjectId
      tenantId: tenant().tenantId
    }
    databases: [
      {
        name: database.name
        availabilityZone: -1
        sku: {
          name: 'GP_S_Gen5_${database.maxVcores}'
          tier: 'GeneralPurpose'
          family: 'Gen5'
          capacity: database.maxVcores
        }
        minCapacity: database.minVcores
        maxSizeBytes: database.maxSizeGiB * 1073741824
        autoPauseDelay: -1
        zoneRedundant: zoneRedundant
        requestedBackupStorageRedundancy: database.backupRedundancy
        backupShortTermRetentionPolicy: { retentionDays: database.pointInTimeRetentionDays }
        backupLongTermRetentionPolicy: database.longTermRetention
        diagnosticSettings: [{ name: 'platform', workspaceResourceId: logs.outputs.resourceId }]
      }
    ]
  }
}

var privateServices = [
  { name: 'sql', id: resourceId('Microsoft.Sql/servers', 'sql${globalStem}'), groupId: 'sqlServer', zoneId: privateDnsZoneResourceIds.sql }
  { name: 'vault', id: resourceId('Microsoft.KeyVault/vaults', 'kv${globalStem}'), groupId: 'vault', zoneId: privateDnsZoneResourceIds.vault }
  { name: 'blob', id: resourceId('Microsoft.Storage/storageAccounts', 'st${globalStem}'), groupId: 'blob', zoneId: privateDnsZoneResourceIds.blob }
  { name: 'registry', id: resourceId('Microsoft.ContainerRegistry/registries', 'acr${globalStem}'), groupId: 'registry', zoneId: privateDnsZoneResourceIds.registry }
]

// Each endpoint depends only on its own target resource (not a combined
// dependsOn across all four) so one service's failure, e.g. SQL blocked by a
// region restriction, does not also block endpoints for services that
// deployed successfully. A loop with a per-item dependsOn ternary is rejected
// by Bicep (BCP176), so these are four explicit module declarations instead.
module privateConnectionSql 'modules/private-endpoint.bicep' = {
  name: '${stem}-sql-private'
  dependsOn: [sql, serviceDnsConfiguration, monitorDnsConfiguration]
  params: {
    name: '${stem}-sql-pe'
    location: location
    tags: tags
    targetResourceId: privateServices[0].id
    groupId: privateServices[0].groupId
    dnsZoneResourceId: privateServices[0].zoneId
    subnetResourceId: privateEndpointSubnetResourceId
  }
}

module privateConnectionVault 'modules/private-endpoint.bicep' = {
  name: '${stem}-vault-private'
  dependsOn: [vault, serviceDnsConfiguration, monitorDnsConfiguration]
  params: {
    name: '${stem}-vault-pe'
    location: location
    tags: tags
    targetResourceId: privateServices[1].id
    groupId: privateServices[1].groupId
    dnsZoneResourceId: privateServices[1].zoneId
    subnetResourceId: privateEndpointSubnetResourceId
  }
}

module privateConnectionBlob 'modules/private-endpoint.bicep' = {
  name: '${stem}-blob-private'
  dependsOn: [keyRing, serviceDnsConfiguration, monitorDnsConfiguration]
  params: {
    name: '${stem}-blob-pe'
    location: location
    tags: tags
    targetResourceId: privateServices[2].id
    groupId: privateServices[2].groupId
    dnsZoneResourceId: privateServices[2].zoneId
    subnetResourceId: privateEndpointSubnetResourceId
  }
}

module privateConnectionRegistry 'modules/private-endpoint.bicep' = {
  name: '${stem}-registry-private'
  dependsOn: [registry, serviceDnsConfiguration, monitorDnsConfiguration]
  params: {
    name: '${stem}-registry-pe'
    location: location
    tags: tags
    targetResourceId: privateServices[3].id
    groupId: privateServices[3].groupId
    dnsZoneResourceId: privateServices[3].zoneId
    subnetResourceId: privateEndpointSubnetResourceId
  }
}

module serviceDnsConfiguration 'modules/private-dns-zone.bicep' = [
  for service in ['sql', 'vault', 'registry']: {
    name: '${stem}-${service}-dns-link'
    scope: resourceGroup(split(privateDnsZoneResourceIds[service], '/')[2], split(privateDnsZoneResourceIds[service], '/')[4])
    params: {
      zoneName: last(split(privateDnsZoneResourceIds[service], '/'))
      tags: tags
      vnetResourceId: virtualNetworkResourceId
      configuration: {
        zone: { mode: 'Existing', resourceId: privateDnsZoneResourceIds[service] }
        link: serviceDnsLinks[service]
        zoneGroupOwner: 'Project'
      }
    }
  }
]

module environment 'br/public:avm/res/app/managed-environment:0.16.0' = {
  params: {
    name: '${stem}-env'
    location: location
    tags: union(tags, acaEnvironmentTags)
    enableTelemetry: false
    infrastructureSubnetResourceId: acaSubnetResourceId
    internal: true
    publicNetworkAccess: 'Disabled'
    zoneRedundant: zoneRedundant
    managedIdentities: { userAssignedResourceIds: [identity.outputs.resourceId] }
    workloadProfiles: [{ name: 'Consumption', workloadProfileType: 'Consumption' }]
    appLogsConfiguration: { destination: 'azure-monitor' }
    diagnosticSettings: [{ name: 'platform', workspaceResourceId: logs.outputs.resourceId }]
  }
}

module environmentDns 'modules/internal-environment-dns.bicep' = {
  params: {
    domain: environment.outputs.defaultDomain
    address: environment.outputs.staticIp!
    virtualNetworkResourceId: virtualNetworkResourceId
    tags: tags
  }
}

module monitorDnsConfiguration 'modules/private-dns-zone.bicep' = [
  for service in monitorServices: {
    name: '${stem}-${service.name}-monitor-dns'
    scope: resourceGroup(
      service.dns.zone.mode == 'Create' ? subscription().subscriptionId : split(service.dns.zone.resourceId, '/')[2],
      service.dns.zone.mode == 'Create' ? resourceGroup().name : split(service.dns.zone.resourceId, '/')[4]
    )
    params: {
      zoneName: service.zone
      tags: tags
      vnetResourceId: virtualNetworkResourceId
      configuration: service.dns
    }
  }
]

module monitorScope 'modules/monitor-private-link-scope.bicep' = if (monitoringAmpls.associationOwner == 'Project') {
  name: '${stem}-monitor-scope'
  scope: resourceGroup(split(monitorAmplsId, '/')[2], split(monitorAmplsId, '/')[4])
  dependsOn: [logs, insights]
  params: {
    name: last(split(monitorAmplsId, '/'))
    tags: tags
    createScope: monitoringAmpls.mode == 'Create'
    workspaceResourceId: resourceId('Microsoft.OperationalInsights/workspaces', '${stem}-logs')
    applicationInsightsResourceId: resourceId('Microsoft.Insights/components', '${stem}-insights')
    workspaceAssociationName: monitoringAmpls.mode == 'Create'
      ? 'workspace-${uniqueString(resourceGroup().id, stem)}'
      : (monitoringAmpls.associationOwner == 'Project' ? monitoringAmpls.workspaceAssociationName! : '')
    applicationInsightsAssociationName: monitoringAmpls.mode == 'Create'
      ? 'insights-${uniqueString(resourceGroup().id, stem)}'
      : (monitoringAmpls.associationOwner == 'Project' ? monitoringAmpls.applicationInsightsAssociationName! : '')
  }
}

module monitorEndpoint 'modules/monitor-private-endpoint.bicep' = if (monitoringPrivateEndpoint.mode == 'Create') {
  name: '${stem}-monitor-private'
  params: {
    name: '${stem}-monitor-pe'
    location: location
    tags: tags
    amplsResourceId: monitorAmplsId
    subnetResourceId: privateEndpointSubnetResourceId
    dnsZoneResourceIds: monitorZoneIds
    manageDnsZoneGroup: monitoringPrivateDns.monitor.zoneGroupOwner == 'Project'
  }
  dependsOn: [monitorScope, monitorDnsConfiguration]
}

output monitoringConnectivity object = {
  amplsResourceId: monitorAmplsId
  privateEndpointResourceId: monitorEndpointId
  privateDnsZoneResourceIds: monitorZoneIds
  workspaceResourceId: logs.outputs.resourceId
  applicationInsightsResourceId: insights.outputs.resourceId
}

output foundation object = {
  managedEnvironmentResourceId: environment.outputs.resourceId
  identityResourceId: identity.outputs.resourceId
  identityClientId: identity.outputs.clientId
  identityPrincipalId: identity.outputs.principalId
  registryLoginServer: registry.outputs.loginServer
  vaultUri: vault.outputs.uri
  sqlHost: sql.outputs.fullyQualifiedDomainName
  databaseName: database.name
  dataProtectionBlobUri: 'https://${keyRing.outputs.name}.blob.${az.environment().suffixes.storage}/data-protection/keys.xml'
  dataProtectionKeyIdentifier: '${vault.outputs.uri}keys/data-protection'
  webOrigin: 'https://${stem}-web.${environment.outputs.defaultDomain}'
}
