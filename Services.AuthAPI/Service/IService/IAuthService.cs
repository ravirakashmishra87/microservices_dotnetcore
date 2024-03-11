using Services.AuthAPI.Models.DTO;

namespace Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);

        Task<bool> AssignRole(string email, string rolename);
    }
}
