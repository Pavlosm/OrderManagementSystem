namespace OrderManagementService.Auth;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string? username, List<string>? roles);
}