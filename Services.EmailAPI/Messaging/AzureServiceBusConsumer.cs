
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Services.EmailAPI.Message;
using Services.EmailAPI.Models.DTO;
using Services.EmailAPI.Services;
using System.Text;

namespace Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string ServiceBusConnectionString;
        private readonly string EmailCartQueueName;
        private readonly string RegisterUserQueueName;
        private readonly string OrderCreatedTopic;
        private readonly string OrderCreatedSubscription;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _emailProcessor;
        private ServiceBusProcessor _registerUserProcessor;
        private ServiceBusProcessor _EmailOrderPlacedProcessor;

        private readonly EmailService _emailService;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            EmailCartQueueName = _configuration.GetValue<string>("TopicOrQueueNames:EmailShoppingCartQueue");
            RegisterUserQueueName = _configuration.GetValue<string>("TopicOrQueueNames:RegisterUserQueue");

            OrderCreatedTopic = _configuration.GetValue<string>("TopicOrQueueNames:OrderCreatedTopic");
            OrderCreatedSubscription = _configuration.GetValue<string>("TopicOrQueueNames:OrderCreatedEmailSubscription");

            var client = new ServiceBusClient(ServiceBusConnectionString);
            _emailProcessor = client.CreateProcessor(EmailCartQueueName);
            _registerUserProcessor = client.CreateProcessor(RegisterUserQueueName);

            _EmailOrderPlacedProcessor = client.CreateProcessor(OrderCreatedTopic, OrderCreatedSubscription);




        }

        public async Task Start()
        {
            _emailProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnRegisterUserRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();

            _EmailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _EmailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            await _EmailOrderPlacedProcessor.StartProcessingAsync();
        }


        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
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

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            var messsage = args.Message;
            var body = Encoding.UTF8.GetString(messsage.Body);

            RewardMessage rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(body);

            try
            {
                await _emailService.LogPlacedOrder(rewardMessage);
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

            await _EmailOrderPlacedProcessor.StopProcessingAsync();
            await _EmailOrderPlacedProcessor.DisposeAsync();
        }
    }
}
