using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MS.MessageBus;
using Services.ProductAPI.Models.DTO;
using Services.ShoppingCartAPI.Data;
using Services.ShoppingCartAPI.Models;
using Services.ShoppingCartAPI.Models.DTO;
using Services.ShoppingCartAPI.Service.IService;
using System.Runtime.CompilerServices;

namespace Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
   // [Authorize]
    public class CartAPIController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public CartAPIController(ApplicationDBContext applicationDBContext, 
                                 IMapper mapper, 
                                 IProductService productService, 
                                 ICouponService couponService,
                                 IMessageBus messageBus,
                                 IConfiguration configuration)
        {
            _dbContext = applicationDBContext;
            this._responseDto = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        /// <summary>
        /// Insert or update cart item
        /// </summary>
        /// <param name="cartDto"></param>
        /// <returns></returns>
        [HttpPost("cartupsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDB = await _dbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDB == null)
                {
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _dbContext.CartHeaders.Add(cartHeader);
                    await _dbContext.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHearderId;
                    _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _dbContext.SaveChangesAsync();

                }
                else
                {
                    var cartDetailsFromDB = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                                                u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                                                u.CartHeaderId == cartHeaderFromDB.CartHearderId);
                    if (cartDetailsFromDB == null)
                    {
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDB.CartHearderId;
                        _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                        _responseDto.Message = "Item added to cart successfuly";
                    }
                    else
                    {
                        cartDto.CartDetails.First().Count += cartDetailsFromDB.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDB.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDB.CartDetailsId;
                        _dbContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                        _responseDto.Message = "Cart updated successfuly";
                    }
                }
                _responseDto.Result = cartDto;
                _responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }


        /// <summary>
        /// Remove items from cart
        /// </summary>
        /// <param name="cartDetailId"></param>
        /// <returns></returns>
        [HttpPost("removecart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailId)
        {
            try
            {
                CartDetails cartDetails = _dbContext.CartDetails.First(u => u.CartDetailsId == cartDetailId);
                if (cartDetails == null) { }
                var cartDetailsCount = _dbContext.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _dbContext.CartDetails.Remove(cartDetails);

                if (cartDetailsCount == 1)
                {
                    _dbContext.CartHeaders.Remove(_dbContext.CartHeaders.First(u => u.CartHearderId == cartDetails.CartHeaderId));
                }
                await _dbContext.SaveChangesAsync();
                _responseDto.IsSuccess = true;
                _responseDto.Result = true;
                _responseDto.Message = "Item removed from cart";

            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpGet("getcart/{userid}")]
        public async Task<ResponseDto> GetCart(string userid)
        {
            try
            {
                CartDto cartDto = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_dbContext.CartHeaders.FirstOrDefault(u => u.UserId == userid))
                };
                cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_dbContext.CartDetails.Where(u => u.CartHeaderId == cartDto.CartHeader.CartHearderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProductsAsync();
                foreach (var item in cartDto.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(p => p.ProductId == item.ProductId);
                    cartDto.CartHeader.CartTotal += item.Count * item.Product.Price;
                }

                if(!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto couponDto = await _couponService.GetCouponAsync(cartDto.CartHeader.CouponCode);
                    if(couponDto != null && cartDto.CartHeader.CartTotal > couponDto.MinimumAmount)
                    {
                        cartDto.CartHeader.CartTotal -= couponDto.DiscountAmount;
                        cartDto.CartHeader.Discount = couponDto.DiscountAmount;
                    }
                }
                _responseDto.Result = cartDto;
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Cart details fetched";
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("applycoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDB = await _dbContext.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDB.CouponCode = cartDto.CartHeader?.CouponCode;
                _dbContext.CartHeaders.Update(cartFromDB);
                await _dbContext.SaveChangesAsync();
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Coupon applied successfuly";

            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("removecoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDB = await _dbContext.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDB.CouponCode = "";
                _dbContext.CartHeaders.Update(cartFromDB);
                await _dbContext.SaveChangesAsync();
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Coupon removed successfuly";

            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("emailcartrequest")]
        public async Task<ResponseDto> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("TopicOrQueueNames:EmailShoppingCartQueue"));
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Email sent successfuly";

            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

    }
}
