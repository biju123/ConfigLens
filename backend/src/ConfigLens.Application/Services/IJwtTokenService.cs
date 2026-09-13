using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ConfigLens.Application.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) IssueToken(string username);
}

/// <summary>
/// Issues short-lived symmetric-key JWTs. Swapping to enterprise auth later
/// (CLAUDE.md section 5) means replacing this registration - or the whole
/// authentication scheme - without changing any controller, since callers
/// only ever depend on ClaimsPrincipal via [Authorize].
/// </summary>
public sealed class JwtTokenService(IConfiguration configuration, IClock clock) : IJwtTokenService
{
    private const string Issuer = "ConfigLens";
    private const string Audience = "ConfigLens";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

    public (string Token, DateTime ExpiresAtUtc) IssueToken(string username)
    {
        var signingKey = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");

        var expiresAtUtc = clock.UtcNow.Add(TokenLifetime);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: [new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, "user")],
            notBefore: clock.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
