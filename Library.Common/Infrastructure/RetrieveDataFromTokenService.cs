
using Library.Core.Models.JwtModels;
using Library.Core.Models.Securities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;

namespace Library.Core.Infrastructure
{
    public interface IRetrieveDataFromTokenService
    {
        CustomClaim GetClaims();
    }

    public class RetrieveDataFromTokenService : IRetrieveDataFromTokenService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtAuthManager _jwtAuthManager;

        public RetrieveDataFromTokenService(IHttpContextAccessor httpContextAccessor, IJwtAuthManager jwtAuthManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtAuthManager = jwtAuthManager;
        }
        public CustomClaim GetClaims()
        {
            string scheme = "", token = "";
            try
            {
                var authorization = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
                if (string.IsNullOrWhiteSpace(authorization))
                    throw new SecurityTokenException("Invalid token");

                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    scheme = headerValue.Scheme;
                    token = headerValue.Parameter;
                }

                var (principal, jwtToken) = _jwtAuthManager.DecodeJwtToken(token);

                CustomClaim customClaim = new CustomClaim
                {
                    DeviceCode = principal.Identity.Name
                };

                return customClaim;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
