using Services.ShoppingCartAPI.Models.DTO;

namespace Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        public Task<CouponDto> GetCouponAsync(string couponCode);
    }
}
