using Microsoft.Extensions.Configuration;

namespace ConfigLens.Application.Services;

public interface ICredentialValidator
{
    bool Validate(string username, string password);
}

/// <summary>
/// MVP-only credential check against a single configured account
/// (CLAUDE.md section 5: hardcoded "user"/"password" for local development,
/// never treated as production authentication). Reads from configuration
/// (env vars CONFIGLENS_AUTH_USERNAME/CONFIGLENS_AUTH_PASSWORD in Docker,
/// defaulted only in appsettings.Development.json) so the values never
/// appear in source and this class can be replaced wholesale by an
/// OIDC-backed validator later without touching AuthController.
/// </summary>
public sealed class HardcodedCredentialValidator(IConfiguration configuration) : ICredentialValidator
{
    public bool Validate(string username, string password)
    {
        var expectedUsername = configuration["Auth:Username"];
        var expectedPassword = configuration["Auth:Password"];

        return !string.IsNullOrEmpty(expectedUsername)
            && !string.IsNullOrEmpty(expectedPassword)
            && string.Equals(username, expectedUsername, StringComparison.Ordinal)
            && string.Equals(password, expectedPassword, StringComparison.Ordinal);
    }
}
