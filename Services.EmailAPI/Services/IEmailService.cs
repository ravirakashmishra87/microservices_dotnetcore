using Services.EmailAPI.Message;
using Services.EmailAPI.Models.DTO;

namespace Services.EmailAPI.Services
{
    public interface IEmailService
    {
        public Task EmailCartAndLog(CartDto cart);
        public Task<bool> LogAndSendEmail(string email, string message);
        public Task<bool> RegisterUserEmailAndLog(string email);
        public Task LogPlacedOrder(RewardMessage rewardMessage);

    }
}
