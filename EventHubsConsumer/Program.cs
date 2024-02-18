using Azure.Identity;
using Azure.Messaging.EventHubs.Consumer;
using EventHubsConsumer;

var builder = Host.CreateApplicationBuilder(args);

var ns = builder.Configuration.GetConnectionString("eventhubns");

builder.Services.AddSingleton(sp =>
{
    return new EventHubConsumerClient(
        EventHubConsumerClient.DefaultConsumerGroupName, 
        ns, 
        "hub", 
        new DefaultAzureCredential());
});

builder.Services.AddHostedService<Consumer>();

var host = builder.Build();
host.Run();
