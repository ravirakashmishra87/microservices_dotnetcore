
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Services.EmailAPI.Models.DTO;
using Services.EmailAPI.Services;
using System.Text;

namespace Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer :IAzureServiceBusConsumer
    {
        private readonly string ServiceBusConnectionString;
        private readonly string EmailCartQueueName;
        private readonly string RegisterUserQueueName;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _emailProcessor;
        private ServiceBusProcessor _registerUserProcessor;
        private readonly EmailService  _emailService;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            EmailCartQueueName = _configuration.GetValue<string>("TopicOrQueueNames:EmailShoppingCartQueue");
            RegisterUserQueueName = _configuration.GetValue<string>("TopicOrQueueNames:RegisterUserQueue");
           
            var client = new ServiceBusClient(ServiceBusConnectionString);
            _emailProcessor = client.CreateProcessor(EmailCartQueueName);
            _registerUserProcessor = client.CreateProcessor(RegisterUserQueueName);

        }

        public async Task Start()
        {
            _emailProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnRegisterUserRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();
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
               await _emailService.EmailCartAndLog(cartDto);
                await args.CompleteMessageAsync(messsage);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task OnRegisterUserRequestReceived(ProcessMessageEventArgs args)
        {
            var messsage = args.Message;
            var body = Encoding.UTF8.GetString(messsage.Body);

            string emailMsg = JsonConvert.DeserializeObject<string>(body);

            try
            {
                await _emailService.RegisterUserEmailAndLog(emailMsg);
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

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();
        }
    }
}
