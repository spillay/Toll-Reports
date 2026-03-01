using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace MIS.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public LoginController(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        // -----------------------------
        // GET: /Login/Login
        // -----------------------------
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Login/Login.cshtml");
        }

        // -----------------------------
        // POST: /Login/Login
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            // ✅ basic validation
            username = (username ?? string.Empty).Trim();
            password = password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return LoginError("Username and password are required.", returnUrl);

            // ✅ build URL safely
            var baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/');
            var endpoint = _config["ApiSettings:AuthLoginEndpoint"]?.Trim();

            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(endpoint))
                return LoginError("Login service is not configured correctly (missing BaseApiUrl/Auth endpoint).", returnUrl);

            // Ensure endpoint starts with /
            if (!endpoint.StartsWith("/")) endpoint = "/" + endpoint;

            var url = $"{baseUrl}{endpoint}";

            // ✅ IMPORTANT: send lowercase JSON keys (most APIs bind these reliably)
            var payloadObj = new { username, password };
            var payload = JsonConvert.SerializeObject(payloadObj);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            string rawBody = "";

            try
            {
                response = await _httpClient.PostAsync(url, content);
                rawBody = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                // Network / DNS / refused connection etc.
                return LoginError($"Login service is not reachable. {ex.Message}", returnUrl);
            }

            // ✅ If API returns non-200, show helpful message
            if (!response.IsSuccessStatusCode)
            {
                // Try extract a message from the API response (if it returns JSON like {message:"..."})
                var apiMessage = TryReadMessage(rawBody);

                // If none, show generic but include status code for debugging
                var msg = string.IsNullOrWhiteSpace(apiMessage)
                    ? $"Invalid username or password. ({(int)response.StatusCode} {response.StatusCode})"
                    : $"{apiMessage} ({(int)response.StatusCode} {response.StatusCode})";

                return LoginError(msg, returnUrl);
            }

            // ✅ Parse success response
            dynamic? data = null;
            try
            {
                data = JsonConvert.DeserializeObject(rawBody);
            }
            catch
            {
                return LoginError("Login succeeded but returned invalid JSON.", returnUrl);
            }

            // ✅ Read fields safely (support camelCase & PascalCase)
            string apiUsername = (string?)data?.username ?? (string?)data?.Username ?? username;
            string firstName = (string?)data?.firstName ?? (string?)data?.FirstName ?? "";
            string lastName = (string?)data?.lastName ?? (string?)data?.LastName ?? "";

            // ✅ Create cookie claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, apiUsername ?? ""),
                new Claim("firstName", firstName ?? ""),
                new Claim("lastName", lastName ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,  // session cookie
                    AllowRefresh = true
                });

            // ✅ redirect back to the report user originally wanted
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Transaction", "Transaction");
        }

        // -----------------------------
        // POST: /Login/Logout
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }

        // -----------------------------
        // GET: /Login/KeepAlive
        // Used by JS to keep the session fresh while active
        // -----------------------------
        [Authorize]
        [HttpGet]
        public IActionResult KeepAlive() => Ok();

        // =============================
        // Helpers
        // =============================
        private IActionResult LoginError(string message, string? returnUrl)
        {
            ViewBag.Error = message;
            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Login/Login.cshtml");
        }

        private static string TryReadMessage(string rawBody)
        {
            if (string.IsNullOrWhiteSpace(rawBody))
                return "";

            // Try to parse { message: "..." } or { Message: "..." }
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(rawBody);
                string msg = (string?)obj?.message ?? (string?)obj?.Message ?? "";
                return msg ?? "";
            }
            catch
            {
                // If it's not JSON, just return empty (don't display raw HTML errors to user)
                return "";
            }
        }
    }
}