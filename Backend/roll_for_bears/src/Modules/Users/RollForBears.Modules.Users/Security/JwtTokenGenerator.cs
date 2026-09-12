using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace RollForBears.Modules.Users.Security;

public class JwtTokenGenerator
{
    private readonly byte[] _secret;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtTokenGenerator(string secret, string issuer, string audience)
    {
        _secret = Convert.FromBase64String(secret);
        _issuer = issuer;
        _audience = audience;
    }

    public string Generate(Guid accountId, string username)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, accountId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, username),
        };
        
        var key = new SymmetricSecurityKey(_secret);

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
            );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}