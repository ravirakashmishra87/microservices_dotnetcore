using MS_Web.Models;
using MS_Web.Models.DTO;
using MS_Web.Service.IService;
using MS_Web.Utility;

namespace MS_Web.Service
{
	public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {

            _baseService = baseService;

        }

        public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.CartAPIBase}api/cart/applycoupon"
            });
        }


        public async Task<ResponseDto?> GetCartByUserAsync(string userid)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CartAPIBase}api/cart/getcart/{userid}"

            });
        }

        public async Task<ResponseDto?> RemoveFromCartAsync(int CartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = CartDetailsId,
                Url = $"{SD.CartAPIBase}api/cart/RemoveCart"
            });
        }

        public async Task<ResponseDto?> SendEmailAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.CartAPIBase}api/cart/emailcartrequest"
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.CartAPIBase}api/cart/cartupsert"
            });
        }
    }
}
