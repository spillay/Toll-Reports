using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models.LoginResponse;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text;

namespace MIS.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<LoginController> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Login/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            username = (username ?? string.Empty).Trim();
            password ??= string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return LoginError("Username and password are required.", returnUrl);

            var loginUrl = BuildLoginUrl();
            if (string.IsNullOrWhiteSpace(loginUrl))
                return LoginError("Login service is not configured correctly (missing BaseApiUrl/Auth endpoint).", returnUrl);

            var result = await CallLoginApiAsync(loginUrl, username, password);

            if (!result.Success)
                return LoginError(result.ErrorMessage, returnUrl);

            var claims = BuildClaims(result.Data!, username);
            var principal = CreatePrincipal(claims);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    AllowRefresh = true
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Transaction", "Transaction");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult KeepAlive() => Ok();

        private string? BuildLoginUrl()
        {
            var baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/');
            var endpoint = _config["ApiSettings:AuthLoginEndpoint"]?.Trim();

            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(endpoint))
                return null;

            if (!endpoint.StartsWith("/"))
                endpoint = "/" + endpoint;

            return $"{baseUrl}{endpoint}";
        }

        private async Task<(bool Success, LoginApiResponse? Data, string ErrorMessage)> CallLoginApiAsync(
    string url,
    string username,
    string password)
        {
            var payload = JsonConvert.SerializeObject(new { username, password });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            string rawBody;

            try
            {
                response = await _httpClient.PostAsync(url, content);
                rawBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Login API response for {Username}: {RawBody}", username, rawBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login API could not be reached.");
                return (false, null, $"Login service is not reachable. {ex.Message}");
            }

            if (!response.IsSuccessStatusCode)
            {
                var apiMessage = TryReadMessage(rawBody);
                var errorMessage = string.IsNullOrWhiteSpace(apiMessage)
                    ? $"Invalid username or password. ({(int)response.StatusCode} {response.StatusCode})"
                    : $"{apiMessage} ({(int)response.StatusCode} {response.StatusCode})";

                return (false, null, errorMessage);
            }

            try
            {
                var obj = JObject.Parse(rawBody);

                var data = new LoginApiResponse
                {
                    Success = ReadBool(obj, "Success", "success"),
                    Message = ReadString(obj, "Message", "message") ?? string.Empty,
                    Username = ReadString(obj, "Username", "username"),
                    FirstName = ReadString(obj, "FirstName", "firstName"),
                    LastName = ReadString(obj, "LastName", "lastName"),
                    Token = ReadString(obj, "Token", "token", "accessToken", "jwt") ?? string.Empty,
                    ExpiresInMinutes = ReadInt(obj, "ExpiresInMinutes", "expiresInMinutes"),
                    RequiresPasswordReset = ReadBool(obj, "RequiresPasswordReset", "requiresPasswordReset"),
                    PasswordExpired = ReadBool(obj, "PasswordExpired", "passwordExpired"),
                    SystemUserId = ReadLongNullable(obj, "SystemUserId", "systemUserId")
                };

                if (string.IsNullOrWhiteSpace(data.Token))
                {
                    return (false, null, $"Login succeeded but no token was returned. Raw API response: {rawBody}");
                }

                return (true, data, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login API returned invalid JSON. Raw body: {RawBody}", rawBody);
                return (false, null, $"Login succeeded but returned invalid JSON. Raw API response: {rawBody}");
            }
        }

        private static List<Claim> BuildClaims(LoginApiResponse data, string fallbackUsername)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, data.Username ?? fallbackUsername),
                new Claim("firstName", data.FirstName ?? string.Empty),
                new Claim("lastName", data.LastName ?? string.Empty),
                new Claim("access_token", data.Token ?? string.Empty),
                new Claim("systemUserId", data.SystemUserId?.ToString() ?? string.Empty)
            };
        }

        private static ClaimsPrincipal CreatePrincipal(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        private IActionResult LoginError(string message, string? returnUrl)
        {
            ViewBag.Error = message;
            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Login/Login.cshtml");
        }

        private static string TryReadMessage(string rawBody)
        {
            if (string.IsNullOrWhiteSpace(rawBody))
                return string.Empty;

            try
            {
                var obj = JObject.Parse(rawBody);
                return ReadString(obj, "Message", "message") ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string? ReadString(JObject obj, params string[] names)
        {
            foreach (var name in names)
            {
                if (obj.TryGetValue(name, out var value))
                    return value?.ToString();
            }

            return null;
        }

        private static bool ReadBool(JObject obj, params string[] names)
        {
            var text = ReadString(obj, names);
            return bool.TryParse(text, out var value) && value;
        }

        private static int ReadInt(JObject obj, params string[] names)
        {
            var text = ReadString(obj, names);
            return int.TryParse(text, out var value) ? value : 0;
        }

        private static long? ReadLongNullable(JObject obj, params string[] names)
        {
            var text = ReadString(obj, names);
            return long.TryParse(text, out var value) ? value : null;
        }
    }
}