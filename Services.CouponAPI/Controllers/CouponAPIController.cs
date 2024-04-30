using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.CouponAPI.Data;
using Services.CouponAPI.Models;
using Services.CouponAPI.Models.DTO;

namespace Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;

        public CouponAPIController(ApplicationDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> couponsList = _dbContext.Coupons.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<CouponDto>>(couponsList);
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Coupon retrived successfuly";
               
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = $"Fail to retrive coupon : {ex.Message}";

            }
            return _responseDto;

        }
        [HttpGet]
       [Route("{id:int}")]
        public object Get(int id)
        {
            try
            {
                Coupon objCoupon = _dbContext.Coupons.First(u=>u.CouponID==id);
                _responseDto.Result = _mapper.Map<CouponDto>(objCoupon);
                _responseDto.IsSuccess = true;
                _responseDto.Message = $"Coupon retrived successfuly for coupnid: {id}";

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = $"Fail to retrive coupon : {ex.Message}";

            }
            return _responseDto;

        }
        [HttpGet]
        [Route("GetByCode/{code}")]
        public object Get(string code)
        {
            try
            {
                Coupon objCoupon = _dbContext.Coupons.First(u => u.CouponCode.ToLower() == code.ToLower());
                _responseDto.Result = _mapper.Map<CouponDto>(objCoupon);
                _responseDto.IsSuccess = true;
                _responseDto.Message = $"Coupon retrived successfuly for coupon code: {code}";

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = $"Fail to retrive coupon : {ex.Message}";

            }
            return _responseDto;

        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public object post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon objCoupon = _mapper.Map<Coupon>(couponDto);
                _dbContext.Add(objCoupon);
                _dbContext.SaveChanges();

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(couponDto.DiscountAmount * 100),
                    Name = couponDto.CouponCode,
                    Currency = "usd",
                    Id = couponDto.CouponCode,
                };
                var service = new Stripe.CouponService();
                service.Create(options);

                _responseDto.Result = _mapper.Map<CouponDto>(objCoupon);
                _responseDto.IsSuccess = true;
                _responseDto.Message = $"Coupon created successfuly.";               

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = $"Fail to create coupon : {ex.Message}";

            }
            return _responseDto;

        }

        [HttpPut]
		[Authorize(Roles = "ADMIN")]
		public object put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon objCoupon = _mapper.Map<Coupon>(couponDto);
                _dbContext.Update(objCoupon);
                _dbContext.SaveChanges();

                _responseDto.Result = _mapper.Map<CouponDto>(objCoupon);
                _responseDto.IsSuccess = true;
                _responseDto.Message = $"Coupon updated successfuly.";

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = $"Fail to update coupon : {ex.Message}";

            }
            return _responseDto;

        }

        [HttpDelete]
        [Route("{id:int}")]
		[Authorize(Roles = "ADMIN")]
		public object delete(int id)
        {
            try
            {
                Coupon objCoupon = _dbContext.Coupons.First(u=>u.CouponID == id);
                _dbContext.Remove(objCoupon);
                _dbContext.SaveChanges();

                var options = new Stripe.CouponCreateOptions
                {                   
                    Id = Convert.ToString(id),
                };
                var service = new Stripe.CouponService();
                service.Delete(objCoupon.CouponCode);

                _responseDto.IsSuccess = true;
                _responseDto.Message = $"Coupon deleted successfuly.";

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = $"Fail to delete coupon : {ex.Message}";

            }
            return _responseDto;

        }
    }
}
