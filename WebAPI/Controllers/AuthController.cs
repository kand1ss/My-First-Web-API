using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.DTO;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/accounts")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private bool ValidateGuid(string? guid)
        => Guid.TryParse(guid, out _);
    
    [HttpPost]
    public async Task<IActionResult> RegisterAccountAsync([FromBody] RegisterDTO registerData)
    {
        try
        {
            await authService.RegisterAsync(registerData);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginIntoAccountAsync([FromBody] LoginDTO loginData)
    {
        string result;
        try
        {
            result = await authService.LoginAsync(loginData);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateAccountAsync([FromBody] UpdateDTO updateData)
    {
        var guid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ValidateGuid(guid))
            return Unauthorized("The token does not contain a user ID");
        
        try
        {
            await authService.UpdateAccountAsync(guid, updateData);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        catch (AccountNotFoundException e)
        {
            return NotFound(e.Message);
        }
        
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        var guid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ValidateGuid(guid))
            return Unauthorized("The token does not contain a user ID");
        
        try
        {
            await authService.DeleteAccountAsync(guid);
        }
        catch (AccountNotFoundException e)
        {
            return NotFound(e.Message);
        }
        
        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetAccountByGuidAsync()
    {
        var guid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ValidateGuid(guid))
            return Unauthorized("The token does not contain a user ID");
        
        AccountDTO result;
        try
        {
            result = await authService.GetAccountByGuidAsync(guid);
        }
        catch (AccountNotFoundException e)
        {
            return NotFound(e.Message);
        }
        
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAccountsAsync()
    {
        IList<AccountDTO> result;
        try
        {
            result = await authService.GetAllAccountsAsync();
        }
        catch (AccountNotFoundException e)
        {
            return NotFound(e.Message);
        }
        
        return Ok(result);
    }
}