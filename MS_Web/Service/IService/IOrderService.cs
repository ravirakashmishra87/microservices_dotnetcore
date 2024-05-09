using MS_Web.Models.DTO;
using MS_Web.Models;

namespace MS_Web.Service.IService
{
    public interface IOrderService
    {
        
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSessionAsync(int OrderMasterId);
        Task<ResponseDto?> GetOrdersAsync(string? userId);
        Task<ResponseDto?> GetOrderAsync(int orderId);
        Task<ResponseDto?> UpdateOrderStatusAsync(int OrderId, string OrderStatus);


    }
}
