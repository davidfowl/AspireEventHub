using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var ns = builder.Configuration.GetConnectionString("eventhubns");

builder.Services.AddSingleton(sp =>
{
    return new EventHubProducerClient(ns, "hub", new DefaultAzureCredential());
});

var app = builder.Build();

app.MapPost("/message", async (Stream body, EventHubProducerClient producerClient) =>
{
    var binaryData = await BinaryData.FromStreamAsync(body);

    await producerClient.SendAsync([new EventData(binaryData)]);

    return Results.Accepted();
});

app.MapDefaultEndpoints();

app.Run();
