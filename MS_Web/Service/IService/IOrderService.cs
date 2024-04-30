using MS_Web.Models.DTO;
using MS_Web.Models;

namespace MS_Web.Service.IService
{
    public interface IOrderService
    {
        
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSessionAsync(int OrderMasterId);
       

    }
}
