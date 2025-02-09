using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Auth;

public class TokenModel
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
}