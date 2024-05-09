using Microsoft.EntityFrameworkCore;
using Services.RewardsAPI.Services;
using Services.RewadsAPI.Data;
using Services.RewardsAPI.Message;
using Services.RewardsAPI.Models;

namespace Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<ApplicationDBContext> _options;
        public RewardService(DbContextOptions<ApplicationDBContext> options)
        {
            _options = options;
        }

        public async Task UpdateRewards(RewardMessage rewardMessage)
        {
            try
            {
                Rewards rewards = new()
                {
                   OrderId = rewardMessage.OrderId,
                   RewardsActivity= rewardMessage.RewardActivity,
                   UserId = rewardMessage.UserId,
                   RewardsDate=DateTime.Now,
                };
                await using var _db = new ApplicationDBContext(_options);
                _db.Rewards.Add(rewards);
                await _db.SaveChangesAsync();
               
            }
            catch (Exception ex)
            {
              
            }
        }
    }
}
