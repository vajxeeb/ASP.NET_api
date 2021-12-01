using Library.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Securites;
using Services;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Library.Core.ViewModels;
using Services.DataService;

namespace WebApi.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IUserSellerService _userService;
        private readonly IOnlineService _onlineService;
        private readonly IMobileVersionService _mobileVersionService;
        private readonly ISaleService _saleService;

        private readonly IJwtAuthManager _jwtAuthManager;

        public AccountController(IUserSellerService userService
            , IJwtAuthManager jwtAuthManager
            , IOnlineService onlineService
            , IMobileVersionService mobileVersionService
            , ISaleService saleService)
        {
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
            _onlineService = onlineService;
            _mobileVersionService = mobileVersionService;
            _saleService = saleService;
        }

        [HttpGet("[action]")]
        public ActionResult Get()
        {
            return Ok("123");
        }

        [HttpGet("[action]/{versionName}"), AllowAnonymous]
        public ActionResult CheckVersion(string versionName)
        {
            return Ok(_mobileVersionService.IsMobileVersionLatest(versionName));
        }
        [HttpGet("[action]/{versionName}/{deviceImei}"), AllowAnonymous]
        public ActionResult CheckVersionV2(string versionName, string deviceImei)
        {
            return Ok(_mobileVersionService.GetVersionDeviceNumberCode(versionName, deviceImei));
        }

        [HttpPost("[action]/{deviceCode}/{password}"), AllowAnonymous]
        public ActionResult Login(string deviceCode, string password)
        {
            if (!_userService.IsValidUserCredentials(deviceCode, password))
                return Unauthorized();

            var user = _userService.GetAsync(t => t.device_code == deviceCode && t.us_pwd == password && t.us_status == 1).Result;

            var role = _userService.GetUserRole();
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, deviceCode.ToString()),
                new Claim(ClaimTypes.NameIdentifier, deviceCode.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(deviceCode.ToString(), claims);

            return Ok(
                new
                {
                    Online = _onlineService.IsSeverOnline(),
                    DeviceCode = deviceCode,
                    Role = role,
                    offlineDate = _onlineService.GetCurrentPeriodOfflineDate(),
                    IsOverMaxSell = _saleService.IsOverMaxSell(deviceCode),
                    jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString,
                    setNumberList= _saleService.GetSellSetNumberList()
                });
        }


        [HttpPost("[action]/{deviceCode}/{oldPassword}/{newPassword}"), AllowAnonymous]
        public IActionResult UserPasswordChange(string deviceCode, string oldPassword, string newPassword)
        {
            Response response = new Response();

            try
            {
                var data = _userService.UserPasswordChange(deviceCode, oldPassword, newPassword);

                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = data;
                response.Data = new object { };
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = Library.Core.ViewModels.StatusCode.InternalServerError;
                response.Message = ex.Message;
                response.Data = new object { };
            }
            return Ok(response);
        }

        [HttpPost("[action]")]
        public ActionResult Logout()
        {
            var userName = User.Identity.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(userName);
            return Ok();
        }

        #region privete method

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        #endregion
    }
}
