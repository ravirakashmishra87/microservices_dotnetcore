using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.MessageBus
{
    public class MessageBus : IMessageBus
    {
        string ConnectionString = "Endpoint=sb://ms-webapp.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c0+/dbB/pn3mmuNUKg1ST+wv6ahYHxlGZ+ASbAqP5c8=";
        public async Task PublishMessage(object message, string topicOrQueueName)
        {
            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(topicOrQueueName);
            var JsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };
            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();

        }
    }
}
