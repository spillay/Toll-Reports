namespace MIS.Web.Models.LoginResponse
{
    public class LoginApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public long? SystemUserId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public bool RequiresPasswordReset { get; set; }
        public bool PasswordExpired { get; set; }

        public string Token { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
    }
}
