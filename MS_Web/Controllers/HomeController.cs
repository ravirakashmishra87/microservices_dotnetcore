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
        
        public HomeController(IProductService productService)
        {
            _productService = productService;

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
