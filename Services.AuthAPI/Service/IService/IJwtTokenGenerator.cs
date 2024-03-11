using Services.AuthAPI.Models;

namespace Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
