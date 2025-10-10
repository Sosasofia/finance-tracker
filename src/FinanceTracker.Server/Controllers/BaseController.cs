using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public abstract class BaseController : ControllerBase
{
    protected bool UserId(out Guid userId)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && Guid.TryParse(claim.Value, out userId))
        {
            return true;
        }

        userId = Guid.Empty;
        return false;
    }
}