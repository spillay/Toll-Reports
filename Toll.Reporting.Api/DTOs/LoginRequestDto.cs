namespace Toll.Reporting.Api.DTOs
{
    public class LoginRequestDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        public long? SystemUserId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public bool RequiresPasswordReset { get; set; }
        public bool PasswordExpired { get; set; }
    }
}