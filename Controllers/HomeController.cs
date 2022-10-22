using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OAuthLine.Controllers;
[Route("line")]
public class HomeController : Controller
{
    protected Service.LineService _lineService { get; set; }
    protected Service.JwtService _jwtService { get; set; }
    public HomeController(Service.LineService lineService,
        Service.JwtService jwtService)
    {
        this._lineService = lineService;
        this._jwtService = jwtService;

    }
    [HttpGet("CallBack")]
    public async Task<IActionResult> CallBack(ViewModels.ViewLineCode loginCode)
    {
        try
        {
            var verifystate = _jwtService.ValidateToken(loginCode.state, out var verifyEx);
            if (!verifystate)
            {
                throw verifyEx;
            }
            var accessTokenData = await _lineService.GetLoginAccessToken(loginCode);
            if (accessTokenData is null)
            {
                throw new Exception("取得accessToken失敗");
            }

            if (_lineService.UpdateLineLogin(accessTokenData, out var identityInfo))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, identityInfo.Name),
                    new Claim(ClaimTypes.Hash, identityInfo.LoginAccessToken),
                    new Claim(ClaimTypes.UserData, identityInfo.LoginIdToken)
                };

                var claim = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claim),
                    new AuthenticationProperties { });
            }
            else
            {
                throw new Exception("更新失敗");
            }
            return LocalRedirect("/");

        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            return LocalRedirect("/?errorType=0");
        }
    }
    [HttpGet("NotifyCallBack")]
    public async Task<IActionResult> NotifyCallBack(ViewModels.ViewLineCode linecode)
    {
        try
        {

            var verifystate = _jwtService.ValidateToken(linecode.state, out var verifyEx);
            if (!verifystate)
            {
                throw verifyEx;
            }
            var accessTokenData = await _lineService.GetNotifyAccessToken(linecode);
            if (string.IsNullOrEmpty(accessTokenData))
            {
                throw new Exception("取得token失敗");
            }
            var idToken = HttpContext.User.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(e => e.Value).FirstOrDefault();
            if(string.IsNullOrEmpty(idToken))
            {
                return LocalRedirect("/line/signOut");
            }
            var IsParseSuccess = _lineService.ParseIdToken(idToken, out var DecodeIdToken);
            if (!IsParseSuccess)
            {
                return LocalRedirect("/line/signOut");
            }
            var sub = DecodeIdToken.Sub;
            if (!_lineService.UpdateLineNotify(sub, accessTokenData))
            {
                throw new Exception("更新錯誤");
            }
            return LocalRedirect("/?successType=2");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return LocalRedirect("/?errorType=2");
        }
    }
    [HttpGet("signOut")]
    public async Task<IActionResult> signOut()
    {


        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return LocalRedirect("/");
    }
}
