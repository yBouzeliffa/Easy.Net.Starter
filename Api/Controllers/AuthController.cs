﻿using Easy.Net.Starter.Api.Authentications;
using Easy.Net.Starter.Extensions;
using Easy.Net.Starter.Models;
using Easy.Net.Starter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Easy.Net.Starter.Api.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] UserCredentials credentials)
        {
            if (await _userService.IsValidUser(credentials.Email, credentials.Password))
            {
                var Id = await _userService.GetIdByEmail(credentials.Email);
                var token = _tokenService.GenerateToken(Id.ToString());
                return Ok(new { token });
            }

            return Unauthorized();
        }

        [HttpGet("hash")]
        [IsAdmin]
        public IActionResult HashTest(string password)
        {
            return Ok(password.HashPassword());
        }
    }
}