namespace OAuthLine.Models;

public class LineIdentity
{
    public int LineIdentityKey { get; set; }
    public string Sub { get; set; } = "";
    public string Name { get; set; } = "";
    public string LoginIdToken { get; set; } = "";
    public string LoginAccessToken { get; set; } = "";
    public string LoginRefreshToken { get; set; } = "";
    public string NotifyAccessToken { get; set; } = "";
    public string Picture { get; set; } = "";
    
}
