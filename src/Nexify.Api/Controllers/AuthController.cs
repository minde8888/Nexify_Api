using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nexify.Domain.Entities.Auth;
using Nexify.Domain.Interfaces;
using Nexify.Service.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Nexify.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly AuthService _authService;
        private readonly TokenValidationParameters _tokenValidationParams;

        public AuthController(
            ITokenService tokenService,
                AuthService authService,
                        TokenValidationParameters tokenValidationParams)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _tokenValidationParams = tokenValidationParams ?? throw new ArgumentNullException(nameof(tokenValidationParams));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Signup([FromBody] Signup user)
        {
            var result = await _authService.CreateUserAsync(user);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResults>> Login([FromBody] Login login)
        {
            var result = await _authService.AuthAsync(login);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("refresh-token")]
        public async Task<ActionResult<AuthResults>> RefreshToken([FromBody] RequestToken tokenRequest)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();

            _tokenValidationParams.ValidateLifetime = false;
            var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);
            _tokenValidationParams.ValidateLifetime = true;

            var response = await _tokenService.VerifyToken(tokenRequest, principal, validatedToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            await _authService.SendResetPassword(forgotPassword, Request.Headers["origin"]);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var result = await _authService.GetPasswordAsync(resetPassword);
            return Ok(result);
        }
    }
}
