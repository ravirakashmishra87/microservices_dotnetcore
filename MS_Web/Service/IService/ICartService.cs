using MS_Web.Models.DTO;
using MS_Web.Models;

namespace MS_Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartByUserAsync(string userid);
        Task<ResponseDto?> UpsertCartAsync(CartDto cartDto);
        Task<ResponseDto?> RemoveFromCartAsync(int CartDetailsId);
        Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto);
        Task<ResponseDto?> SendEmailAsync(CartDto cartDto);
       

    }
}
