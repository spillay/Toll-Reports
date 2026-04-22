using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;


namespace Toll.Reporting.Api.Controllers
{

    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(
            IAuthRepository repo,
            IJwtTokenService jwtTokenService,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _repo = repo;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username and password are required.");

            var result = await _repo.LoginAsync(req.Username, req.Password);

            if (!result.Success)
                return Unauthorized(result);

            result.Token = _jwtTokenService.GenerateToken(result);

            result.ExpiresInMinutes = int.TryParse(_configuration["JwtSettings:ExpiryMinutes"], out var minutes)
                ? minutes
                : 120;

            return Ok(result);
        }
    }
}