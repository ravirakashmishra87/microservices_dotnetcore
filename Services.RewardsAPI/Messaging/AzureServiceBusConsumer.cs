
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Services.RewardAPI.Services;
using Services.RewardsAPI.Message;
using System.Text;

namespace Services.RewardsAPI.Messaging
{
    public class AzureServiceBusConsumer :IAzureServiceBusConsumer
    {
        private readonly string ServiceBusConnectionString;
        private readonly string OrderCreatedTopic;
        private readonly string OrderCreatedRewardSubscription;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _rewardProcessor;
        private readonly RewardService  _rewardService;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            _rewardService = rewardService;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            OrderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueName:OrderCreatedTopic");
            OrderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueName:OrderCreatedRewardSubscription");
           
            var client = new ServiceBusClient(ServiceBusConnectionString);
            _rewardProcessor = client.CreateProcessor(OrderCreatedTopic, OrderCreatedRewardSubscription);
           

        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();            
        }

        
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private  async Task OnNewRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var messsage = args.Message;
            var body = Encoding.UTF8.GetString(messsage.Body);

            RewardMessage objMessage = JsonConvert.DeserializeObject<RewardMessage>(body);

            try
            {
               await _rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(messsage);
            }
            catch (Exception ex)
            {

                throw;
            }
        }      

        public async Task Stop()
        {
          await _rewardProcessor.StopProcessingAsync();
           await _rewardProcessor.DisposeAsync();
        }
    }
}
