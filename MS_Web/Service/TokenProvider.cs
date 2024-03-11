using MS_Web.Service.IService;
using MS_Web.Utility;

namespace MS_Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }
        public void ClearToken()
        {
           _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookieName);
        }

        public string? GetToken()
        {
            string? token =string.Empty;
           bool? hasToken= _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookieName, out token);
            return hasToken is true ?token:null;
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookieName, token);
        }
    }
}
