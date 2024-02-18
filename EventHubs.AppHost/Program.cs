var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureProvisioning();

var eventHub = builder.AddEventHubs("eventhubns", ["hub"]);

builder.AddProject<Projects.EventHubsConsumer>("consumer")
    .WithReference(eventHub);

builder.AddProject<Projects.EventHubsApi>("api")
    .WithReference(eventHub);

builder.Build().Run();
