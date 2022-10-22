using Microsoft.AspNetCore.Components;

namespace OAuthLine.Pages;

public class LineBase : ComponentBase
{
    [Inject]
    public Models.OAuthLineContext _context { get; set; }
    [Inject]
    public IHttpClientFactory ClientFactory { get; set; }
    [Inject]
    public IConfiguration config { get; set; }
    [Inject]
    public NavigationManager _navigation { get; set; }
    [Inject]
    public Service.JwtService _jwtService { get; set; }
    [Inject]
    public Service.LineService _lineService { get; set; }
}
