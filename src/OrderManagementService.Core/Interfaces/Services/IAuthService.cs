using OrderManagementService.Core.Entities;

namespace OrderManagementService.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string username, string password);
    Task<AuthResult> RegisterAsync(string email, string password, Role role);
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Error { get; set; }
} 