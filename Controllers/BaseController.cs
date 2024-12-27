using Easy.Net.Starter.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Easy.Net.Starter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateApiKey]
    [ValidateReferer]
    [Authorize]
    public class BaseController : ControllerBase
    {
    }
}
