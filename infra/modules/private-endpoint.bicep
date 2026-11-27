param name string
param location string
param tags object
param subnetResourceId string
param targetResourceId string
param groupId string
param dnsZoneResourceId string

resource endpoint 'Microsoft.Network/privateEndpoints@2024-05-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    subnet: { id: subnetResourceId }
    privateLinkServiceConnections: [
      {
        name: groupId
        properties: {
          privateLinkServiceId: targetResourceId
          groupIds: [groupId]
        }
      }
    ]
  }
}

resource zoneGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2024-05-01' = {
  parent: endpoint
  name: 'default'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: groupId
        properties: { privateDnsZoneId: dnsZoneResourceId }
      }
    ]
  }
}
