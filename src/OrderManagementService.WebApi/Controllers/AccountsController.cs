using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementService.Auth;
using OrderManagementService.Core.Entities;
using OrderManagementService.WebApi.Dtos;

namespace OrderManagementService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AccountsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountsController> _logger;
    private readonly ITokenService _tokenService;
    
    public AccountsController(
        UserManager<ApplicationUser> userManager, 
        ILogger<AccountsController> logger, 
        ITokenService tokenService)
    {
        _userManager = userManager;
        _logger = logger;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email
        };
        
        var existingUser = await _userManager.FindByNameAsync(model.Email);
        if (existingUser != null)
        {
            return BadRequest("User already exists");
        }

        if (!Role.AllValidRegistrationRoles.Contains(model.Role))
        {
            return BadRequest("Invalid role");
        }
        
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        result = await _userManager.AddToRoleAsync(user: user, role: model.Role);

        if (result.Succeeded)
        {
            return Ok();
        }
        
        return BadRequest(result.Errors);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
    {
        try
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return BadRequest("User with this username is not registered with us.");
            }
            
            var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (isValidPassword == false)
            {
                return Unauthorized();
            }
            
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateAccessToken(user.Id, user.UserName, userRoles.ToList());
            return Ok(new TokenModel { AccessToken = token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "An error occurred while trying to login");
        }
    }
}