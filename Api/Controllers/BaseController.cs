using Easy.Net.Starter.Models;
using Easy.Net.Starter.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Easy.Net.Starter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateApiKey]
    [ValidateReferer]
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected ConnectedUser GetConnectedUser()
        {
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;

            ArgumentNullException.ThrowIfNull(identity, "L'identité de l'utilisateur n'est pas disponible.");

            var user = new ConnectedUser().FromClaims(identity);

            return user;
        }

        protected ActionResult<T> OpeResponse<T>(OpeResult<T> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode((int)result.GetHttpStatusCode(), result.Error);
        }
    }
}
