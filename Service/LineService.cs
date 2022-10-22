using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OAuthLine.Service;

public class LineService
{
    protected Models.OAuthLineContext _context { get; set; }
    protected IHttpClientFactory ClientFactory { get; set; }
    protected readonly IConfiguration Configuration;
    public LineService(Models.OAuthLineContext context, 
        IHttpClientFactory clientFactory,
        IConfiguration configurationBuilder)
    {
        _context = context;
        ClientFactory = clientFactory;
        Configuration = configurationBuilder;
    }
    /// <summary>
    /// 取得lineLogin的token
    /// </summary>
    /// <param name="lineLoginCode"></param>
    /// <returns></returns>
    public async Task<ViewModels.ViewLineLoginToken?> GetLoginAccessToken(ViewModels.ViewLineCode lineLoginCode)
    {
        try
        {
            var callback = Configuration.GetValue<string>("Auth:LineLogin:CallBackUrl");
            var clientId = Configuration.GetValue<string>("Auth:LineLogin:Channel_ID"); 
            var clientSecrect = Configuration.GetValue<string>("Auth:LineLogin:Channel_secret");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/oauth2/v2.1/token");
            Dictionary<string, string> formDataDictionary = new()
            {
                {"code", lineLoginCode.code },
                {"grant_type", "authorization_code" },
                { "redirect_uri", callback},
                { "client_id", clientId},
                { "client_secret", clientSecrect},
            };
            var formData = new FormUrlEncodedContent(formDataDictionary);
            request.Content = formData;
            var client = ClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("取得token失敗");
            }

            var strResult = await response.Content.ReadAsStreamAsync();
            var data = JsonSerializer.Deserialize<ViewModels.ViewLineLoginToken>(strResult);
            
            return data;
        }
        catch(Exception ex)
        {    
            return null;
        }
    }
    /// <summary>
    /// 取得lineNotify的token
    /// </summary>
    /// <param name="lineCode"></param>
    /// <returns></returns>
    public async Task<string> GetNotifyAccessToken(ViewModels.ViewLineCode lineCode)
    {
        try
        {
            var callback = Configuration.GetValue<string>("Auth:LineNotify:CallBackUrl");
            var clientId = Configuration.GetValue<string>("Auth:LineNotify:Client_ID");
            var clientSecrect = Configuration.GetValue<string>("Auth:LineNotify:Client_Secret");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://notify-bot.line.me/oauth/token");
            Dictionary<string, string> formDataDictionary = new()
            {
                { "grant_type", "authorization_code" },
                { "code", lineCode.code },
                { "redirect_uri", callback },
                { "client_id", clientId },
                { "client_secret", clientSecrect },
            };
            var formData = new FormUrlEncodedContent(formDataDictionary);
            request.Content = formData;
            var client = ClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("取得token失敗");
            }
            var strResult = await response.Content.ReadAsStreamAsync();
            var data = JsonSerializer.Deserialize<ViewModels.ViewLineNotifyToken>(strResult);
            return data.AccessToken;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public bool ParseIdToken(string token, out ViewModels.IdToken idToken)
    {
        try
        {
            var paloadStr = token.Split('.')[1];
            var result = JsonSerializer.Deserialize<ViewModels.IdToken>(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(paloadStr)));
            if (result is null)
            {
                throw new Exception("解析失敗");
            }
            idToken = result;
            return true;

        }
        catch(Exception ex)
        { 
            idToken = null;
            return false; 
        }
    }

    public bool UpdateLineLogin(ViewModels.ViewLineLoginToken LoginToken, out Models.LineIdentity identity)
    {
        try
        {
            if (!this.ParseIdToken(LoginToken.IdToken, out var idToken))
            {
                throw new Exception("token解析錯誤");
            }

            var user = _context.LineIdentity.Where(e => e.Sub == idToken.Sub).FirstOrDefault();
            if (user == null)
            {
                user = new Models.LineIdentity
                {
                    Sub = idToken.Sub,
                    Name = idToken.Name,
                    Picture = idToken.Picture,
                    LoginIdToken = LoginToken.IdToken,
                    LoginAccessToken = LoginToken.AccessToken,
                    LoginRefreshToken = LoginToken.RefreshToken,
                };
                _context.LineIdentity.Add(user);
            }
            else
            {
                user.LoginIdToken = LoginToken.IdToken;
                user.LoginAccessToken = LoginToken.AccessToken;
                user.LoginRefreshToken = LoginToken.RefreshToken;
            }
            _context.SaveChanges();
            identity = user;
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            identity = null;
            return false;
        }
    }
    /// <summary>
    /// 更新linenotify資訊
    /// </summary>
    /// <param name="Sub"></param>
    /// <param name="AccessToken"></param>
    /// <returns></returns>
    public bool UpdateLineNotify(string Sub, string AccessToken)
    {
        try
        {
            var user = _context.LineIdentity.Where(e => e.Sub == Sub).FirstOrDefault();
            if (user != null)
            {
                user.NotifyAccessToken = AccessToken; 
            }
            else
            {
                
                throw new Exception("無此人資料");
            }
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
    /// <summary>
    /// 撤銷notify連結
    /// </summary>
    /// <param name="Sub"></param>
    /// <returns></returns>
    public async Task<bool> RevokeLineNotify(string Sub)
    {
        try
        {
            var user = _context.LineIdentity.Where(e => e.Sub == Sub).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("找不到使用者");
            }
            var accesstoken = user.NotifyAccessToken.ToString();
            if (string.IsNullOrEmpty(accesstoken))
            {
                return true;
            }
            var request = new HttpRequestMessage(HttpMethod.Post, "https://notify-api.line.me/api/revoke");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesstoken);
            var client = ClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("解除綁定失敗");
            }

            user.NotifyAccessToken = "";
            _context.SaveChanges();
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
        
    }

    public async Task<bool> SendNotify(string message)
    {
        try
        {
            var Id = Guid.NewGuid();
            var now = DateTime.UtcNow.AddHours(8);
            _context.LineNotifySendMt.Add(new Models.LineNotifySendMt
            {
                LineNotifySendMtId = Id,
                Message = message,
                SendTime = now
            });
            _context.SaveChanges();
            var sendUser = _context.LineIdentity.Where(e => !string.IsNullOrEmpty(e.NotifyAccessToken)).ToList();
            foreach (var item in sendUser)
            {
                var detailTime = DateTime.UtcNow.AddHours(8);
                var data = new Models.LineNotifySendDt
                {
                    LineNotifySendMtId = Id,
                    LineIdentityKey = item.LineIdentityKey,
                    SendTime = now,
                };
                data.isSuccess = await this.SendNotifyMessage(item.NotifyAccessToken, message);
                _context.LineNotifySendDt.Add(data);
            }
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> SendNotifyMessage(string accessToken, string message)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://notify-api.line.me/api/notify");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            Dictionary<string, string> formDataDictionary = new()
            {
                { "message", message },
            };
            var formData = new FormUrlEncodedContent(formDataDictionary);
            request.Content = formData;
            var client = ClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("傳送失敗");
            }
            var strResult = await response.Content.ReadAsStreamAsync();
            var data = JsonSerializer.Deserialize<ViewModels.ViewLineStatus>(strResult);
            if (data != null && data.status == 200)
            {
                return true;
            }
            return false;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
}
