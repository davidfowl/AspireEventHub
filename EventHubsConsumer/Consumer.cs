﻿using System.Text;
using Azure.Messaging.EventHubs.Consumer;

namespace EventHubsConsumer;

internal class Consumer(EventHubConsumerClient client, ILogger<Consumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        void LogString(string message) => logger.Log(LogLevel.Information, 0, message, null, (s, _) => s);

        await foreach (var partition in client.ReadEventsAsync(stoppingToken))
        {
            LogString(partition.Data.EventBody.ToString());
        }
    }
}
