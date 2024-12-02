using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenGenerator
{
    private const string SecretKey = @"5OmAI2mdOY4nMKUHZpUiwIKhXRpx8/et2YBWxSlwsSQb338Y/v1tfIy7HnCevoTV\r\n"; // Should be stored securely
    private const int TokenExpiryMinutes = 60; // Token expiration time

    public static string GenerateToken(string userId, string? roleId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);

        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
        };

        if (roleId != null) {
            //Console.WriteLine("Adding Role");
            claims = claims.Append(new Claim(ClaimTypes.Role, roleId)).ToArray();
        }

        //Console.WriteLine(claims.Length);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(TokenExpiryMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
