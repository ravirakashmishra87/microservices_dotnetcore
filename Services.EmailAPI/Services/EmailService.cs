using Microsoft.EntityFrameworkCore;
using Services.EmailAPI.Data;
using Services.EmailAPI.Models;
using Services.EmailAPI.Models.DTO;
using System.Text;

namespace Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<ApplicationDBContext> _options;
        public EmailService(DbContextOptions<ApplicationDBContext> options)
        {
            _options = options;
        }
        public async Task EmailCartAndLog(CartDto cart)
        {
           StringBuilder message = new StringBuilder();
            message.AppendLine("<br/> Cart email request");
            message.AppendLine("<br/> Total:" +cart.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach(var item in cart.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name+ "x "+ item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");
          await  LogAndSendEmail(cart.CartHeader.Email,message.ToString());
        }

        public async Task<bool> LogAndSendEmail(string email, string message)
        {
            try
            {
                EmailLogger emailLogger = new()
                {
                    Email = email,
                    Message = message,
                    EmailSent=  DateTime.Now
                };
                await using var _db = new ApplicationDBContext(_options);
                _db.EmailLoggers.Add(emailLogger);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<bool> RegisterUserEmailAndLog(string email)
        {
            string message = $"User email registration is successful {email}";
           return await LogAndSendEmail(email, message);
        }
    }
}
