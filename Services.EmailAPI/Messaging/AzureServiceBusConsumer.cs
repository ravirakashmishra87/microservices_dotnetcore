
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Services.EmailAPI.Models.DTO;
using System.Text;

namespace Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer :IAzureServiceBusConsumer
    {
        private readonly string ServiceBusConnectionString;
        private readonly string EmailCartQueueName;

        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _emailProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            EmailCartQueueName = _configuration.GetValue<string>("TopicOrQueueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(ServiceBusConnectionString);
            _emailProcessor = client.CreateProcessor(EmailCartQueueName);


        }

        public async Task Start()
        {
            _emailProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailProcessor.StartProcessingAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private  async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var messsage = args.Message;
            var body = Encoding.UTF8.GetString(messsage.Body);

            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                await args.CompleteMessageAsync(messsage);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task Stop()
        {
          await _emailProcessor.StopProcessingAsync();
           await _emailProcessor.DisposeAsync();
        }
    }
}
