param domain string
param address string
param virtualNetworkResourceId string
param tags object

resource zone 'Microsoft.Network/privateDnsZones@2024-06-01' = {
  name: domain
  location: 'global'
  tags: tags
}

resource link 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2024-06-01' = {
  parent: zone
  name: 'application-network'
  location: 'global'
  tags: tags
  properties: {
    registrationEnabled: false
    virtualNetwork: { id: virtualNetworkResourceId }
  }
}

resource records 'Microsoft.Network/privateDnsZones/A@2024-06-01' = [
  for recordName in ['@', '*']: {
    parent: zone
    name: recordName
    properties: {
      ttl: 300
      aRecords: [{ ipv4Address: address }]
    }
  }
]
