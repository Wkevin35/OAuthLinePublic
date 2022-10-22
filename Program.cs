using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<OAuthLine.Models.OAuthLineContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnect")));
builder.Services.AddScoped<OAuthLine.Models.OAuthLineContext>();
builder.Services.AddScoped<OAuthLine.Service.LineService>();
builder.Services.AddScoped<OAuthLine.Service.JwtService>();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
});
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
var app = builder.Build();
using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
var db = serviceProvider.GetService<OAuthLine.Models.OAuthLineContext>();
db.Database.Migrate();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
