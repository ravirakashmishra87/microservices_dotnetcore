using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

namespace Services.CouponAPI.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAuthentication( this WebApplicationBuilder builder) {


            var secrete = builder.Configuration.GetValue<string>("APISettings:APISecrete");
            var issuer = builder.Configuration.GetValue<string>("APISettings:Issuer");
            var audience = builder.Configuration.GetValue<string>("APISettings:Audience");

            var key = Encoding.ASCII.GetBytes(secrete);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateAudience = true
                };
            });
            return builder;
        }
    }
}
