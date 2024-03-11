using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MS_Web.Models;
using MS_Web.Models.DTO;
using MS_Web.Service.IService;
using MS_Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MS_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;

        }

        [HttpGet]
        public IActionResult login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }
        [HttpPost]
        public async Task<IActionResult> login(LoginRequestDto model)
        {
            ResponseDto response = await _authService.LoginAsync(model);
            if (response != null && response.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
                _tokenProvider.SetToken(loginResponseDto.Token);
                await SignInUser(loginResponseDto);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // ModelState.AddModelError("CustomError",response.Message); return View(model);
                TempData["Error"] = response.Message;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> register(RegistrationRequestDto model)
        {
            ResponseDto response = await _authService.RegisterAsync(model);

            ResponseDto assignRoleResp;
            if (response != null && response.IsSuccess)
            {
                assignRoleResp = await _authService.AsignRoleAsync(model);
                if (assignRoleResp != null && assignRoleResp.IsSuccess)
                {
                    TempData["Success"] = "Registration successful";
                    return RedirectToAction(nameof(login));
                }
            }
            else
            {
                TempData["Error"] = response.Message;
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View();
        }


        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();

            return RedirectToAction("Index", "home");
        }

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));


            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));


            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
