using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IJwtTokenService
    {
        string GenerateToken(LoginResponseDto user);
    }
}
