using Microsoft.AspNetCore.Mvc;
using MS_Web.Models.DTO;
using MS_Web.Models;
using MS_Web.Service.IService;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace MS_Web.Controllers
{
	public class CouponController : Controller
	{
		private readonly ICouponService _couponService;

		public CouponController(ICouponService couponService)
		{
			_couponService = couponService;
		}
		public async Task<IActionResult> CouponIndex()
		{
			List<CouponDto> couponDtosList = new();
			ResponseDto? response = await _couponService.GetAllCouponAsync();
			if (response != null && response.IsSuccess)
			{
				couponDtosList = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return View(couponDtosList);
		}

		public IActionResult CouponCreate()
		{

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CouponCreate(CouponDto couponDto)
		{
			if (ModelState.IsValid)
			{
				ResponseDto? response = await _couponService.CreateCouponAsync(couponDto);
				if (response != null && response.IsSuccess)
				{
					TempData["Success"] = response.Message;
					return RedirectToAction(nameof(CouponIndex));
				}
				else { TempData["Error"] = response?.Message; }
			}
			return RedirectToAction(nameof(CouponCreate));
		}


		public async Task<IActionResult> CouponDelete(int couponId)
		{
			ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);
			if (response != null && response.IsSuccess)
			{
				CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else { TempData["Error"] = response?.Message; }
			return RedirectToAction(nameof(CouponIndex));
		}

		[HttpPost]
		public async Task<IActionResult> CouponDelete(CouponDto couponDto)
		{
			ResponseDto? response = await _couponService.DeleteCouponAsync(couponDto.CouponID);
			if (response != null && response.IsSuccess)
			{
				TempData["Success"] = "Coupon deleted successfully";
				return RedirectToAction(nameof(CouponIndex));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return View(couponDto);

		}
	}
}
