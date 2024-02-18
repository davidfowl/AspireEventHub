@description('Specifies a project name that is used to generate the Event Hub name and the Namespace name.')
@minLength(6)
param eventHubNamespaceName string

param principalId string
param principalType string

param eventHubs array = []

@description('Specifies the Azure location for all resources.')
param location string = resourceGroup().location

@description('Specifies the messaging tier for Event Hub Namespace.')
@allowed([
    'Basic'
    'Standard'
])
param eventHubSku string = 'Basic'

var resourceToken = uniqueString(resourceGroup().id)

resource eventHubNamespace 'Microsoft.EventHub/namespaces@2023-01-01-preview' = {
    name: '${eventHubNamespaceName}${resourceToken}'
    location: location
    sku: {
        name: eventHubSku
        tier: eventHubSku
        capacity: 1
    }
    properties: {
        isAutoInflateEnabled: false
        maximumThroughputUnits: 0
    }

    resource hub 'eventhubs' = [for name in eventHubs: {
        name: name
        properties: {
            messageRetentionInDays: 1
            partitionCount: 1
        }
    }
    ]
}

// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#azure-event-hubs-data-owner

resource eventHubRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
    name: guid(eventHubNamespace.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'f526a384-b230-433a-b45c-95f59c4a2dec'))
    scope: eventHubNamespace
    properties: {
        principalId: principalId
        principalType: principalType
        roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'f526a384-b230-433a-b45c-95f59c4a2dec')
    }
}

output serviceBusEndpoint string = eventHubNamespace.properties.serviceBusEndpoint