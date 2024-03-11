using MS_Web.Models.DTO;
using MS_Web.Models;

namespace MS_Web.Service.IService
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetCouponAsync(string couponCode);
        Task<ResponseDto?> GetAllCouponAsync();
        Task<ResponseDto?> GetCouponByIdAsync(int id);
        Task<ResponseDto?> CreateCouponAsync(CouponDto coupon);
        Task<ResponseDto?> DeleteCouponAsync(int id);
        Task<ResponseDto?> UpdateCouponAsync(CouponDto coupon);

    }
}
