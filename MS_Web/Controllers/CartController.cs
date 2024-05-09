using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS_Web.Models;
using MS_Web.Models.DTO;
using MS_Web.Service.IService;
using MS_Web.Utility;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MS_Web.Controllers
{

    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await UserCartDetails());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await UserCartDetails());
        }


        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            if (ModelState.IsValid)
            {
                CartDto objCartDto = await UserCartDetails();

                objCartDto.CartHeader.Email = cartDto.CartHeader?.Email;
                objCartDto.CartHeader.Phone = cartDto.CartHeader?.Phone;
                objCartDto.CartHeader.Name = cartDto.CartHeader?.Name;

                var response = await _orderService.CreateOrderAsync(objCartDto);

                OrderMasterDto orderMasterDto = JsonConvert.DeserializeObject<OrderMasterDto>(Convert.ToString(response.Result));
                if (response != null && response.IsSuccess)
                {
                    //create stripe session

                    var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                    StripeRequestDto stripeRequestDto = new()
                    {
                        ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderMasterDto.OrderMasterId,
                        CancelUrl = domain + "cart/checkout",
                        orderMasterDto = orderMasterDto
                    };

                    var _response = await _orderService.CreateStripeSessionAsync(stripeRequestDto);
                    StripeRequestDto stripeResponse = JsonConvert.DeserializeObject<StripeRequestDto>
                                                        (Convert.ToString(_response.Result));

                    Response.Headers.Add("location", stripeResponse.StripeSessionUrl);
                    return new StatusCodeResult(303);
                }
            }
            return View();
        }

        // [Authorize]
        public async Task<ActionResult> Confirmation(int orderId)
        {
            ResponseDto responseDto = await _orderService.ValidateStripeSessionAsync(orderId);
            OrderMasterDto ordermasterdto = JsonConvert.DeserializeObject<OrderMasterDto>(Convert.ToString(responseDto.Result));
            if (ordermasterdto.Status == SD.Status_Approved)
            {
                return View(orderId);
            }
            return View(orderId);
        }
        public async Task<CartDto> UserCartDetails()
        {
            var userID = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponseDto? response = await _cartService.GetCartByUserAsync(userID);
            if (response != null && response.IsSuccess)
            {
                CartDto cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cart;
            }
            return new CartDto();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Coupon applied successfuly";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Coupon removed successfuly";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> RemoveItem(int cartDetailsId)
        {
            // var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await UserCartDetails();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

            ResponseDto response = await _cartService.SendEmailAsync(cart);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
