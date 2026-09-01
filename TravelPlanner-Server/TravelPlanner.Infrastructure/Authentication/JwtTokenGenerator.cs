using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TravelPlanner.Application.Common.Interfaces;

namespace TravelPlanner.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;

    /* הזרקת הקונפיגורציה כדי שנוכל לקרוא את המפתח מה-appsettings.json */
    public JwtTokenGenerator(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(int userId, string firstName)
    {
        /* 1. הגדרת המפתח הסודי מתוך ההגדרות שכתבנו קודם */
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        /* 2. הגדרת ה-Claims: המידע שמוטמע בתוך הטוקן (כמו שם ו-ID) */
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, firstName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        /* 3. יצירת האובייקט של הטוקן עם תוקף ומנפיק */
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryInMinutes"])),
            signingCredentials: creds
        );

        /* 4. המרת אובייקט הטוקן למחרוזת טקסט ארוכה */
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}