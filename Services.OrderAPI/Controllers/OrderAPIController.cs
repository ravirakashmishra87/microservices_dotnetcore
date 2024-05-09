using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MS.MessageBus;
using Services.OrderAPI.Data;
using Services.OrderAPI.Models;
using Services.OrderAPI.Models.DTO;
using Services.OrderAPI.Service.IService;
using Services.OrderAPI.Utility;
using Stripe;
using Stripe.Checkout;

namespace Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly ResponseDto _Response;
        private IMapper _Mapper;
        private readonly ApplicationDBContext _dbContext;
        private readonly IProductService _productService;
        private readonly IMessageBus _messagebus;
        private readonly IConfiguration _configuration;

        public OrderAPIController(IMapper mapper, ApplicationDBContext dBContext, IProductService productService, IConfiguration configuration, IMessageBus messageBus)
        {
            _dbContext = dBContext;
            _Mapper = mapper;
            _productService = productService;
            this._Response = new ResponseDto();
            _configuration = configuration;
            _messagebus = messageBus;
        }

        [Authorize]
        [HttpPost("createorder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cart)
        {
            try
            {
                OrderMasterDto orderMasterDto = _Mapper.Map<OrderMasterDto>(cart.CartHeader);
                orderMasterDto.OrderTime = DateTime.Now;
                orderMasterDto.Status = SD.Status_Pending;
                orderMasterDto.OrderDetails = _Mapper.Map<IEnumerable<OrderDetailsDto>>(cart.CartDetails);
                orderMasterDto.OrderTotal = Math.Round(orderMasterDto.OrderTotal, 2);

                OrderMaster ObjOrderMaster = _dbContext.OrderMaster.Add(_Mapper.Map<OrderMaster>(orderMasterDto)).Entity;
                await _dbContext.SaveChangesAsync();
                orderMasterDto.OrderMasterId = ObjOrderMaster.OrderMasterId;

                _Response.Result = orderMasterDto;
                _Response.IsSuccess = true;
                _Response.Message = "Order created successfuly";

            }
            catch (Exception ex)
            {

                _Response.IsSuccess = false;
                _Response.Message = ex.Message;
            }
            return _Response;

        }

        [Authorize]
        [HttpGet("getorders")]
        public async Task<ResponseDto> GetOrders(string? userid = "")
        {
            try
            {
                IEnumerable<OrderMaster> objOrderMaster;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objOrderMaster = await _dbContext.OrderMaster.Include(u => u.OrderDetails).
                                        OrderByDescending(u => u.OrderMasterId).ToListAsync();
                }
                else
                {
                    objOrderMaster = await _dbContext.OrderMaster.Include(u => u.OrderDetails).
                                        Where(u => u.UserId == userid).
                                        OrderByDescending(u => u.UserId == userid).ToListAsync();
                }
                _Response.IsSuccess = true;
                _Response.Result = _Mapper.Map<IEnumerable<OrderMasterDto>>(objOrderMaster);
            }
            catch (Exception ex)
            {
                _Response.IsSuccess = false;
                _Response.Message = ex.Message;
            }
            return _Response;
        }

        [Authorize]
        [HttpGet("getorder/{id:int}")]
        public async Task<ResponseDto> GetOrder(int id)
        {
            try
            {
                OrderMaster ordermaster = await _dbContext.OrderMaster.
                                            Include(u => u.OrderDetails).
                                            FirstAsync(u => u.OrderMasterId == id);
                _Response.Result = _Mapper.Map<OrderMasterDto>(ordermaster);
                _Response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _Response.IsSuccess = false;
                _Response.Message = ex.Message;
            }
            return _Response;
        }

        [Authorize]
        [HttpPost("updateorderstatus/{id:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int id, [FromBody] string NewOrderStatu)
        {
            try
            {
                OrderMaster objOrderMaster = await _dbContext.OrderMaster.FirstAsync(u => u.OrderMasterId == id);
                objOrderMaster.Status = NewOrderStatu;

                if (objOrderMaster != null)
                {
                    if (NewOrderStatu == SD.Status_Cancelled)
                    {
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = objOrderMaster.PaymentIntentId,
                        };
                        var service = new RefundService();
                        Refund refund = service.Create(options);

                    }
                    objOrderMaster.Status = NewOrderStatu;
                    await _dbContext.SaveChangesAsync();
                    _Response.Result = objOrderMaster;
                    _Response.IsSuccess = true;
                    _Response.Message = "Order Status updated";
                }
            }
            catch (Exception ex)
            {

                _Response.IsSuccess = false;
                _Response.Message = ex.Message;
            }
            return _Response;
        }

        [Authorize]
        [HttpPost("createstripesession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var discountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.orderMasterDto.CouponCode
                    }
                };
                foreach (var item in stripeRequestDto.orderMasterDto.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                            }
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDto.orderMasterDto.Discount > 0)
                {
                    options.Discounts = discountObj;
                }
                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                stripeRequestDto.StripeSessionId = session.Id;

                OrderMaster orderMaster = _dbContext.OrderMaster.First(u => u.OrderMasterId == stripeRequestDto.orderMasterDto.OrderMasterId);
                orderMaster.StripeSessionId = session.Id;
                _dbContext.SaveChanges();
                _Response.Result = stripeRequestDto;

            }
            catch (Exception ex)
            {
                _Response.IsSuccess = false;
                _Response.Message = ex.Message;

            }
            return _Response;
        }

        [Authorize]
        [HttpPost("validatestripesession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int OrderMasterId)
        {
            try
            {
                OrderMaster objOrderMaster = _dbContext.OrderMaster.First(o => o.OrderMasterId == OrderMasterId);

                var service = new SessionService();
                Session session = await service.GetAsync(objOrderMaster.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);
                if (paymentIntent != null && paymentIntent.Status == "succeeded")
                {
                    //PAYMENT SUCCESSFUL
                    objOrderMaster.PaymentIntentId = paymentIntent.Id;
                    objOrderMaster.Status = SD.Status_Approved;
                    _dbContext.SaveChanges();

                    RewardDto rewardDto = new RewardDto
                    {
                        OrderId = objOrderMaster.OrderMasterId,
                        RewardActivity = Convert.ToInt32(objOrderMaster.OrderTotal),
                        UserId = objOrderMaster.UserId
                    };

                    string TopicName = _configuration.GetValue<string>("TopicOrQueueNames:OrderCreatedTopic");
                    await _messagebus.PublishMessage(rewardDto, TopicName);

                    _Response.Result = _Mapper.Map<OrderMasterDto>(objOrderMaster);
                }

            }
            catch (Exception ex)
            {
                _Response.IsSuccess = false;
                _Response.Message = ex.Message;

            }
            return _Response;
        }
    }
}
