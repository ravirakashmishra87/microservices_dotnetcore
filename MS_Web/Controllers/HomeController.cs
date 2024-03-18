using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS_Web.Models;
using MS_Web.Models.DTO;
using MS_Web.Service;
using MS_Web.Service.IService;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MS_Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        
        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;

        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> productDtosList = new();
            ResponseDto? response = await _productService.GetAllProductAsync();
            if (response != null && response.IsSuccess)
            {
                productDtosList = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }
            return View(productDtosList);
        }
        [Authorize]
        public async Task<IActionResult> Details(int Id)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(Id);
            if (response != null && response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ActionName("details")]
        public async Task<IActionResult> Details(ProductDto productDto)
        {
            CartDto cartDto = new CartDto()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetail = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId,
            };
            List<CartDetailsDto> cartDetailsDtos = new() { cartDetail };
            cartDto.CartDetails = cartDetailsDtos;

            ResponseDto? response = await _cartService.UpsertCartAsync(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Item added to cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }
            return View(productDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
