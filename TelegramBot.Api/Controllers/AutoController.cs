using Microsoft.AspNetCore.Mvc;
using TelegramBot.App.DTO;
using TelegramBot.App.Interfaces;
using TelegramBot.Domain.Models;

namespace TelegramBot.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AutoController : ControllerBase
{
    private IAutoService _autoService;
    
    public AutoController(IAutoService autoService)
    {
        _autoService = autoService;
    }

    [HttpGet("{autoId:guid}")]
    public async Task<IActionResult> GetAuto([FromRoute] Guid autoId)
    {
        if (autoId == Guid.Empty)
        {
            return BadRequest("The autoId parameter cannot be empty.");
        }

        var response = await _autoService.GetAutoAsync(autoId);
        if (response == null)
        {
            return NotFound($"Auto with ID {autoId} not found.");
        }

        return Ok(response);
    }


    [HttpGet("ListOfAutos/{personId:guid}")]
    public async Task<IActionResult> GetListOfAutos([FromRoute] Guid personId)
    {
        var response = await _autoService.GetAutosAsync(personId);
        return Ok(response);
    }

    [HttpPost("Auto")]
    public async Task AddAuto([FromBody] Auto auto)
    {
        await _autoService.AutoAddAsync(auto);
    }

    [HttpPut("Auto")]
    public async Task UpdateAuto([FromBody] Auto auto)
    {
        await _autoService.UpdateAutoAsync(auto);
    }

    [HttpDelete("Auto/{id}")]
    public async Task DeleteAuto([FromRoute] Auto auto)
    {
        await _autoService.AutoDeleteAsync(auto);
    }
}