using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application;
using Application.DTO;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/accounts")]
public class AuthController(IAuthService authService, IOptions<AuthSettings> settings) : ControllerBase
{
    private bool ValidateGuid(string? guid)
        => Guid.TryParse(guid, out _);
    
    [HttpPost]
    public async Task<IActionResult> RegisterAccountAsync([FromBody] RegisterDTO registerData)
    {
        await authService.RegisterAsync(registerData);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginIntoAccountAsync([FromBody] LoginDTO loginData)
    {
        var token = await authService.LoginAsync(loginData);
        
        HttpContext.Response.Cookies.Append("authToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(settings.Value.TokenLifetime)
        });
        
        return Ok(new { message = "Logged in successfully" });
    }
    
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateAccountAsync([FromBody] UpdateDTO updateData)
    {
        var guid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ValidateGuid(guid))
            return Unauthorized("The token does not contain a user ID");
        
        await authService.UpdateAccountAsync(guid, updateData);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        var guid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ValidateGuid(guid))
            return Unauthorized("The token does not contain a user ID");
        
        await authService.DeleteAccountAsync(guid);
        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetAccountByGuidAsync()
    {
        var guid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ValidateGuid(guid))
            return Unauthorized("The token does not contain a user ID");
        
        var result = await authService.GetAccountByGuidAsync(guid);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAccountsAsync()
    {
        var result = await authService.GetAllAccountsAsync();
        return Ok(result);
    }
}