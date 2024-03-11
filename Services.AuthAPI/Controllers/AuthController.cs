using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AuthAPI.Models.DTO;
using Services.AuthAPI.Service.IService;

namespace Services.AuthAPI.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            string resposneMsg = await _authService.Register(registrationRequestDto);
            _responseDto.Message = resposneMsg;
            if (resposneMsg != "OK")
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = resposneMsg;
                return BadRequest(_responseDto);
            }

            _responseDto.IsSuccess = true;
            return Ok(_responseDto);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.Login(loginRequestDto);
            if (loginResponse.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Username or Password is invalid";
                return BadRequest(_responseDto);
            }

            _responseDto.IsSuccess = true;
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> assignRole([FromBody] RegistrationRequestDto model)
        {
            var AssignRoleStatus =await _authService.AssignRole(model.Email, model.RoleName.ToUpper());
            if(!AssignRoleStatus)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Fail to assign role";
                return BadRequest(_responseDto);
            }
            _responseDto.IsSuccess = true;
            _responseDto.Message = "Role assigned successfuly";
            return Ok(_responseDto);
        }

    }
}
