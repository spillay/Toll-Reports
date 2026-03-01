using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IAuthRepository
    {
        Task<LoginResponseDto> LoginAsync(string username, string password);
    }
}