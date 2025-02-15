using Easy.Net.Starter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Easy.Net.Starter.Api.Controllers
{
    public class AppController : BaseController
    {
        private readonly AppSettings _appSettings;
        public AppController(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        [HttpGet("version")]
        [AllowAnonymous]
        public IActionResult GetVersion()
        {
            return Ok(_appSettings.Version);
        }
    }
}
