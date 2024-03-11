using Microsoft.AspNetCore.Identity;
using Services.AuthAPI.Data;
using Services.AuthAPI.Models;
using Services.AuthAPI.Models.DTO;
using Services.AuthAPI.Service.IService;

namespace Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtGenerator;
        public AuthService(ApplicationDBContext applicationDBContext,
            IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _applicationDBContext = applicationDBContext;
            _jwtGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string rolename)
        {
            var user = _applicationDBContext.applicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(rolename).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(rolename)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user,rolename);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _applicationDBContext.applicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || isValid == false)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = string.Empty,
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            UserDto userDto = new()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Id = user.Id,
                Name = user.UserName,
            };
            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = _jwtGenerator.GenerateToken(user, roles),
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    //var userToReturn = _applicationDBContext.applicationUsers.Find(user.UserName);
                    //UserDto userdto = new()
                    //{
                    //    Id = userToReturn.Id,
                    //    Email = userToReturn.Email,
                    //    Name = userToReturn.Name,
                    //    PhoneNumber = userToReturn.PhoneNumber,

                    //};
                    return "OK";
                }
                else
                {
                    return $"Fail to register : {result.Errors.FirstOrDefault().Description}";
                }
            }
            catch (Exception ex)
            {

                return $"Exception : {ex.Message}";
            }
        }
    }
}
