using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.AuthAPI.Models;
using Services.AuthAPI.Service.IService;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.AuthAPI.Service
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JWTOptions _jWTOptions;
        public JwtTokenGenerator(IOptions<JWTOptions> jWTOptions)
        {
            _jWTOptions = jWTOptions.Value;
        }
        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            var TokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jWTOptions.APISecrete);

            var claimList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
                new Claim(JwtRegisteredClaimNames.Name, applicationUser.Name)
            };
            claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jWTOptions?.Audience,
                Issuer = _jWTOptions?.Issuer,
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = TokenHandler.CreateToken(TokenDescriptor);
            return TokenHandler.WriteToken(token);
        }
    }
}
