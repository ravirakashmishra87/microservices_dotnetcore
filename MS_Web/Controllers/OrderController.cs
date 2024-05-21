using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using MS_Web.Models;
using MS_Web.Models.DTO;
using MS_Web.Service.IService;
using MS_Web.Utility;
using Newtonsoft.Json;

namespace MS_Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> orderDetails(int orderId)
        {
            OrderMasterDto ordermaster = new OrderMasterDto();
            string UserId = UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
           
            ResponseDto response =await _orderService.GetOrderAsync(orderId);
            if (response != null && response.IsSuccess)
            {
                ordermaster =  JsonConvert.DeserializeObject<OrderMasterDto>(Convert.ToString(response.Result));
            }
            if(!User.IsInRole(SD.RoleAdmin) && UserId != ordermaster.UserId)
            {
                return NotFound();
            }

            return View(ordermaster);
        }

       
        [HttpGet]
        public IActionResult GetAllOrders(string status)
        {
            IEnumerable<OrderMasterDto> orderMasterList ;
            string UserId=string.Empty;
            if(!User.IsInRole(SD.RoleAdmin))
            {
                UserId = User.Claims.Where(u=>u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            ResponseDto response =  _orderService.GetOrdersAsync(UserId).GetAwaiter().GetResult();
            if(response != null && response.IsSuccess) {
                orderMasterList = JsonConvert.DeserializeObject<List<OrderMasterDto>>(Convert.ToString(response.Result));
                switch(status)
                {
                    case "approved":
                        orderMasterList = orderMasterList.Where(u => u.Status == SD.Status_Approved);
                        break;

                    case "readyforpickup":
                        orderMasterList = orderMasterList.Where(u => u.Status == SD.Status_ReadyForPickup);
                        break;

                    case "cancelled":
                        orderMasterList = orderMasterList.Where(u => u.Status == SD.Status_Cancelled || u.Status == SD.Status_Refunded);
                        break;

                    default: 
                        break;
                }
            }
            else
            {
                orderMasterList = new List<OrderMasterDto>();
            }
            return Json(new {data = orderMasterList});
        }

        
        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var response = await _orderService.UpdateOrderStatusAsync(orderId, SD.Status_ReadyForPickup);
           
            if(response != null && response.IsSuccess) {
                TempData["Success"] = "Status updated sucessfuly";
                return RedirectToAction(nameof(orderDetails), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatusAsync(orderId, SD.Status_Completed);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Status updated sucessfuly";
                return RedirectToAction(nameof(orderDetails), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatusAsync(orderId, SD.Status_Cancelled);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Status updated sucessfuly";
                return RedirectToAction(nameof(orderDetails), new { orderId = orderId });
            }
            return View();
        }
    }
}
