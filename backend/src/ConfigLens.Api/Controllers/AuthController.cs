using ConfigLens.Application.Auth;
using ConfigLens.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(ICredentialValidator credentialValidator, IJwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        if (!credentialValidator.Validate(request.Username, request.Password))
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, type: "authentication-failure", title: "Invalid username or password.");
        }

        var (token, expiresAtUtc) = jwtTokenService.IssueToken(request.Username);
        return Ok(new LoginResponse(token, expiresAtUtc, request.Username));
    }
}
