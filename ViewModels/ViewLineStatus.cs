namespace OAuthLine.ViewModels;

public enum loginError
{
    登入失敗=0,
    時間逾時=1,
    綁定Notify失敗 = 2,
    解除綁定Notify失敗 = 3,
}

public enum loginSuccess
{
    登入成功 = 1,
    綁定Notify成功 = 2,
    解除綁定Notify成功 = 3
}

public class ViewLineStatus
{
    public int status { get; set; }
    public string message { get; set; } = "";
}