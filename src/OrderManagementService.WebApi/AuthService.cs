// using System.Security.Cryptography;
// using System.Text;
// using Microsoft.Extensions.Options;
// using OrderManagementService.Core.Interfaces;
// using OrderManagementService.Core.Interfaces.Repositories;
// using OrderManagementService.Core.Interfaces.Services;
// using OrderManagementService.Core.Models;
//
// namespace OrderManagementService.WebApi;
//
// public class AuthService : IAuthService
// {
//     private readonly IUserRepository _userRepository;
//     private readonly JwtSettings _jwtSettings;
//
//     public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
//     {
//         _userRepository = userRepository;
//         _jwtSettings = jwtSettings.Value;
//     }
//
//     public async Task<AuthResult> LoginAsync(string email, string password)
//     {
//         var user = await _userRepository.GetByEmailAsync(email);
//         if (user == null)
//             return new AuthResult { Success = false, Error = "User not found" };
//
//         if (!VerifyPasswordHash(password, user.PasswordHash))
//             return new AuthResult { Success = false, Error = "Invalid password" };
//
//         var token = GenerateJwtToken(user);
//         return new AuthResult { Success = true, Token = token };
//     }
//
//     public async Task<AuthResult> RegisterAsync(string email, string password, UserRole role)
//     {
//         if (await _userRepository.GetByEmailAsync(email) != null)
//             return new AuthResult { Success = false, Error = "Email already exists" };
//
//         var passwordHash = HashPassword(password);
//         var user = new ApplicationUser
//         {
//             Id = Guid.NewGuid(),
//             Email = email,
//             UserName = email,
//             PasswordHash = passwordHash,
//             Role = role
//         };
//
//         await _userRepository.CreateAsync(user);
//         var token = GenerateJwtToken(user);
//         return new AuthResult { Success = true, Token = token };
//     }
//
//     private string GenerateJwtToken(ApplicationUser user)
//     {
//         // var claims = new[]
//         // {
//         //     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//         //     new Claim(ClaimTypes.Email, user.Email),
//         //     new Claim(ClaimTypes.Role, user.Role.ToString())
//         // };
//         //
//         // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
//         // var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//         //
//         // var token = new JwtSecurityToken(
//         //     issuer: _jwtSettings.Issuer,
//         //     audience: _jwtSettings.Audience,
//         //     claims: claims,
//         //     expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
//         //     signingCredentials: credentials
//         // );
//         //
//         // return new JwtSecurityTokenHandler().WriteToken(token);
//         return "12345";
//     }
//
//     private string HashPassword(string password)
//     {
//         using var hmac = new HMACSHA512();
//         var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
//         return Convert.ToBase64String(hash);
//     }
//
//     private bool VerifyPasswordHash(string password, string storedHash)
//     {
//         using var hmac = new HMACSHA512();
//         var computedHash = Convert.ToBase64String(
//             hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
//         return computedHash == storedHash;
//     }
// }
