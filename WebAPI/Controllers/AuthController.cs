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
    public async Task<IActionResult> RegisterAccountAsync([FromBody] AccountRegisterDTO registerData)
    {
        await authService.RegisterAsync(registerData);
        return Ok("Account registered");
    }

    private void AppendTokensToCookies(TokensDTO tokens)
    {
        HttpContext.Response.Cookies.Append("accessToken", tokens.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(settings.Value.AccessTokenLifetime)
        });
        HttpContext.Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(settings.Value.RefreshTokenLifetime)
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginIntoAccountAsync([FromBody] AccountLoginDTO accountLoginData)
    {
        var tokens = await authService.LoginAsync(accountLoginData);
        
        AppendTokensToCookies(tokens);
        
        return Ok("Logged in successfully");
    }

    [HttpPost("login/refresh")]
    public async Task<IActionResult> RefreshLoginAsync()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest("Refresh token not found in cookies or it is empty.");
        
        var tokens = await authService.LoginWithTokenAsync(refreshToken);
        
        AppendTokensToCookies(tokens);
        
        return Ok("Logged in successfully");
    }
    
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateAccountAsync([FromBody] AccountUpdateDTO updateData)
    {
        var guid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ValidateGuid(guid))
            return Unauthorized("The token does not contain a user ID");
        
        await authService.UpdateAccountAsync(guid, updateData);
        return Ok("Account updated");
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        var guid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ValidateGuid(guid))
            return Unauthorized("The token does not contain a user ID");
        
        await authService.DeleteAccountAsync(guid);
        return Ok("Account deleted");
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
    [Authorize]
    public async Task<IActionResult> GetAllAccountsAsync()
    {
        var result = await authService.GetAllAccountsAsync();
        return Ok(result);
    }
}