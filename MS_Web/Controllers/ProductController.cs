using Microsoft.AspNetCore.Mvc;
using MS_Web.Models.DTO;
using MS_Web.Models;
using MS_Web.Service.IService;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace MS_Web.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
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

		public IActionResult Create()
		{

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(ProductDto productDto)
		{
			if (ModelState.IsValid)
			{
				ResponseDto? response = await _productService.CreateProductAsync(productDto);
				if (response != null && response.IsSuccess)
				{
					TempData["Success"] = response.Message;
					return RedirectToAction(nameof(Index));
				}
				else { TempData["Error"] = response?.Message; }
			}
            return View(productDto);
        }


		public async Task<IActionResult> Delete(int Id)
		{
			ResponseDto? response = await _productService.GetProductByIdAsync(Id);
			if (response != null && response.IsSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else { TempData["Error"] = response?.Message; }
            return NotFound();
        }

		[HttpPost]
		public async Task<IActionResult> Delete(ProductDto productDto)
		{
			ResponseDto? response = await _productService.DeleteProductAsync(productDto.ProductId);
			if (response != null && response.IsSuccess)
			{
				TempData["Success"] = "product deleted successfully";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return View(productDto);
		}

        public async Task<IActionResult> Edit(int Id)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(Id);

            if (response != null && response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
		public async Task<IActionResult> Edit(ProductDto productDto)
		{
			if (ModelState.IsValid)
			{
				ResponseDto? response = await _productService.UpdateProductAsync(productDto);
				if (response != null && response.IsSuccess)
				{
					TempData["Success"] = "product details updated successfully";
					return RedirectToAction(nameof(Index));
				}
				else
				{
					TempData["Error"] = response?.Message;
				}
			}
			return View(productDto);
		}
	}
}
