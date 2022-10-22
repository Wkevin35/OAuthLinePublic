namespace OAuthLine.Models;

public class LineNotifySendDt
{
    public int LineNotifySendDtKey { get; set; }
    public Guid LineNotifySendMtId { get; set; }
    public int LineIdentityKey { get; set; }
    public bool isSuccess { get; set; }
    public DateTime SendTime { get; set; }
    #region Notmapped
    public string Name { get; set; } = "";
    public string Pic { get; set; } = "";
    #endregion
}
