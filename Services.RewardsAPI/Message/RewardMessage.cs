﻿namespace Services.RewardsAPI.Message
{
    public class RewardMessage
    {
        public string UserId { get; set; }
        public int RewardActivity { get; set; }
        public int OrderId { get; set; }
    }
}
