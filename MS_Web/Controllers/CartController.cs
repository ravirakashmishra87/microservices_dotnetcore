using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS_Web.Models;
using MS_Web.Models.DTO;
using MS_Web.Service.IService;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MS_Web.Controllers
{
    
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await UserCartDetails());
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
            if (response != null && response.IsSuccess) {
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
    }
}
