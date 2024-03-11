using MS_Web.Models;
using MS_Web.Models.DTO;

namespace MS_Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto> AsignRoleAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
    }
}
