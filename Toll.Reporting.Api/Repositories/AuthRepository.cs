using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private const int ReporterRoleId = 6;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LoginResponseDto> LoginAsync(string username, string password)
        {
            username = (username ?? "").Trim();
            password = password ?? "";

            var user = await _context.SystemUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            if (!user.IsActive)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Account is inactive."
                };
            }

            if (user.IsLocked)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Account is locked."
                };
            }

            if (user.ActivationDate > DateTime.Now)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Account not yet activated."
                };
            }

            if (user.PasswordExpires &&
                (!user.PasswordExpiryDate.HasValue || user.PasswordExpiryDate.Value <= DateTime.Now))
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Password expired.",
                    PasswordExpired = true
                };
            }

            if ((user.Password ?? "") != password)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            // Check whether this user has the Reporter role
            var hasReporterRole = await _context.SystemUserRoles
                .AsNoTracking()
                .AnyAsync(x => x.SystemUserId == user.SystemUserId && x.RoleId == ReporterRoleId);

            if (!hasReporterRole)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "You are not authorized to access this system."
                };
            }

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful.",
                SystemUserId = user.SystemUserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RequiresPasswordReset = user.RequiresPasswordReset,
                PasswordExpired = false
            };
        }
    }
}