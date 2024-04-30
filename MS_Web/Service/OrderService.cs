using MS_Web.Models;
using MS_Web.Models.DTO;
using MS_Web.Service.IService;
using MS_Web.Utility;

namespace MS_Web.Service
{
	public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {

            _baseService = baseService;

        }

        public async Task<ResponseDto?> CreateOrderAsync(CartDto cartDto)
        {
            var response = await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.OrderAPIBase}api/order/createorder"
            });
            return response;
        }

        public  async Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto)
        {
            var response = await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDto,
                Url = $"{SD.OrderAPIBase}api/order/createstripesession"
            });
            return response;
        }

        public async Task<ResponseDto?> ValidateStripeSessionAsync(int OrderMasterId)
        {
            var response = await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = OrderMasterId,
                Url = $"{SD.OrderAPIBase}api/order/validatestripesession"
            });
            return response;
        }
    }
}
