targetScope = 'resourceGroup'

import { FileShares } from 'modules/contracts.bicep'

@minLength(2)
@maxLength(6)
param prefix string

@minLength(2)
@maxLength(3)
param environmentName string

@description('Four existing approved SMB shares; account keys are owner-populated Key Vault secrets, never parameter values.')
param fileShares FileShares

var stem = '${prefix}-${environmentName}-csg'
var globalStem = '${prefix}${environmentName}csg${take(uniqueString(resourceGroup().id, prefix, environmentName, 'csg'), 10)}'
var shares = [
  { config: fileShares.templates, accessMode: 'ReadOnly' }
  { config: fileShares.data, accessMode: 'ReadWrite' }
  { config: fileShares.pdfArchive, accessMode: 'ReadWrite' }
  { config: fileShares.pdfOutbound, accessMode: 'ReadWrite' }
]

resource environment 'Microsoft.App/managedEnvironments@2025-07-01' existing = {
  name: '${stem}-env'
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' existing = {
  name: '${stem}-id'
}

resource vault 'Microsoft.KeyVault/vaults@2024-11-01' existing = {
  name: 'kv${globalStem}'
}

resource registrations 'Microsoft.App/managedEnvironments/storages@2025-07-01' = [
  for share in shares: {
    parent: environment
    name: share.config.registrationName
    properties: {
      azureFile: {
        accountName: share.config.accountName
        shareName: share.config.shareName
        accessMode: share.accessMode
        accountKeyVaultProperties: {
          identity: identity.id
          keyVaultUrl: '${vault.properties.vaultUri}secrets/${share.config.accountKeySecretName}'
        }
      }
    }
  }
]

output registrationNames string[] = [for share in shares: share.config.registrationName]
