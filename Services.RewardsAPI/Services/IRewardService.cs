

using Services.RewardsAPI.Message;

namespace Services.RewardsAPI.Services
{
    public interface IRewardService
    {
        public Task UpdateRewards(RewardMessage rewardMessage);
      

    }
}
