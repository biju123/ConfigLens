namespace ConfigLens.Application.Auth;

public sealed record LoginResponse(string Token, DateTime ExpiresAtUtc, string Username);
