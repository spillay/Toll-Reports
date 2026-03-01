using Microsoft.AspNetCore.Mvc;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;

namespace Toll.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthRepository repo, ILogger<AuthController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username and password are required.");

            var result = await _repo.LoginAsync(req.Username, req.Password);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}