using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Application.Features.Users.Commands.ChangePassword;
using FinanceTracker.Application.Features.Users.Commands.DeleteAccount;
using FinanceTracker.Application.Features.Users.Commands.UpdateUserPreferences;
using FinanceTracker.Application.Features.Users.Commands.UpdateUserProfile;
using FinanceTracker.Application.Features.Users.Models;
using FinanceTracker.Application.Features.Users.Queries.GetCurrentUser;
using FinanceTracker.Application.Features.Users.Queries.GetUserPreferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/users/me")]
[EnableRateLimiting("fixed")]
[Authorize]
public class UserPreferencesController : ControllerBase
{
    private readonly IRequestDispatcher _mediator;

    public UserPreferencesController(IRequestDispatcher mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserResponse>> GetUserProfile()
    {
        var query = new GetCurrentUserQuery();
        var response = await _mediator.Send(query);

        return Ok(response);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<UserResponse>> UpdateUserProfile([FromBody] UpdateUserProfileCommand command)
    {
        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpPut("password")]
    public async Task<ActionResult<Unit>> UpdateUserPassword([FromBody] ChangePasswordCommand command)
    {
        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUserAccount()
    {
        var command = new DeleteAccountCommand();
        await _mediator.Send(command);

        return Ok();
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<UserPreferencesResponse>> GetUserPreferences()
    {
        var query = new GetUserPreferencesQuery();
        var response = await _mediator.Send(query);

        return Ok(response);
    }

    [HttpPut("preferences")]
    public async Task<ActionResult<UserPreferencesResponse>> UpdateUserPreferences([FromBody] UpdateUserPreferencesCommand command)
    {
        var response = await _mediator.Send(command);

        return Ok(response);
    }
}
