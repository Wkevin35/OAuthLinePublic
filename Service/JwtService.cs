using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OAuthLine.Service;

public class JwtService
{
    private readonly IConfiguration _config;
    private string issuer = "";
    private string signKey = "";
    public JwtService(IConfiguration config)
    {
        _config = config;
        this.issuer = _config.GetValue<string>("Auth:JWT:Issuer");
        this.signKey = _config.GetValue<string>("Auth:JWT:SignKey");
    }
    public string GenerateToken()
    {
        var expires = DateTime.UtcNow.AddMinutes(10);
        var securityKey = this.getSecurityKey();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(),
            Issuer = this.issuer,
            SigningCredentials = new SigningCredentials(securityKey,
                                                        SecurityAlgorithms.HmacSha256Signature),
            Expires = expires,
            NotBefore = DateTime.UtcNow,
        };

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.CreateToken(tokenDescriptor);
        var token = handler.WriteToken(securityToken);

        return token;
    }

    public bool ValidateToken(string token, out Exception validateErrorException)
    {

        var securityKey = this.getSecurityKey();

        var tokenHandler = new JwtSecurityTokenHandler();

        var parameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidIssuer = this.issuer,
            IssuerSigningKey = securityKey,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var isAuthor = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
            validateErrorException = null;
            return true;
        }
        catch (Exception ex)
        {
            validateErrorException = ex;
            return false;
        }
    }

    private SymmetricSecurityKey getSecurityKey()
    {
        var secretBytes = Convert.FromBase64String(this.signKey);
        var securityKey = new SymmetricSecurityKey(secretBytes);
        return securityKey;
    }
}
