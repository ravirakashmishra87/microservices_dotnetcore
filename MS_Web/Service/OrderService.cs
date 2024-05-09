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

        public async Task<ResponseDto?> GetOrderAsync(int orderId)
        {
            var response = await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,               
                Url = $"{SD.OrderAPIBase}api/order/getorder/{orderId}"
            });
            return response;
        }

        public async Task<ResponseDto?> GetOrdersAsync(string? userId)
        {
            var response = await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.OrderAPIBase}api/order/getorders/{userId}"
            });
            return response;
        }

        public async Task<ResponseDto?> UpdateOrderStatusAsync(int OrderId, string OrderStatus)
        {
            var response = await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,

                Data= OrderStatus,
                Url = $"{SD.OrderAPIBase}api/order/updateorderstatus/{OrderId}"
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
