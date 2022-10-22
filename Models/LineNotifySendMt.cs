namespace OAuthLine.Models;

public class LineNotifySendMt
{
    public int LineNotifySendMtKey { get; set; }
    public Guid LineNotifySendMtId { get; set; }
    public string Message { get; set; }
    public DateTime SendTime { get; set; }
}
