using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace RollForBears.Modules.Users.Security;

public class RefreshTokenGenerator
{
    public string Generate()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
        
        return Base64UrlEncoder.Encode(randomBytes);
    }

    public string Hash(string token)
    {
        byte[] tokenBytes = Base64UrlEncoder.DecodeBytes(token);
        byte [] hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToBase64String(hashBytes);
    }
}