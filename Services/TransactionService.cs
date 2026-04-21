using Microsoft.Extensions.Configuration;
using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MIS.Web.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionService(
            HttpClient httpClient,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        private string BuildQuery(
            IDictionary<string, string?> singles,
            IDictionary<string, IEnumerable<string>?>? multiples = null)
        {
            var sb = new StringBuilder();

            foreach (var kv in singles)
            {
                if (!string.IsNullOrWhiteSpace(kv.Value))
                {
                    sb.Append(kv.Key);
                    sb.Append('=');
                    sb.Append(Uri.EscapeDataString(kv.Value.Trim()));
                    sb.Append('&');
                }
            }

            if (multiples != null)
            {
                foreach (var kv in multiples)
                {
                    if (kv.Value == null) continue;

                    foreach (var v in kv.Value.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        sb.Append(kv.Key);
                        sb.Append('=');
                        sb.Append(Uri.EscapeDataString(v.Trim()));
                        sb.Append('&');
                    }
                }
            }

            return sb.ToString().TrimEnd('&');
        }

        private (string baseUrl, string endpoint) GetApiInfo(string endpointKey)
        {
            var baseUrl = _config["BaseApiUrl:Link"]?.TrimEnd('/');
            var endpoint = _config[endpointKey];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseApiUrl:Link missing in config.");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException($"{endpointKey} missing in config.");

            if (!endpoint.StartsWith("/"))
                endpoint = "/" + endpoint;

            return (baseUrl, endpoint);
        }

        private Dictionary<string, IEnumerable<string>?> BuildMultiFilters(TransactionInputModel model)
        {
            return new Dictionary<string, IEnumerable<string>?>
            {
                { "operationalShift", model.SelectedShifts },
                { "tollOperators",    model.SelectedTollOperators },
                { "laneNames",        model.SelectedLanes },
                { "paymentMethods",   model.SelectedPaymentMethods }
            };
        }

        private void AddBearerToken(HttpRequestMessage request)
        {
            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("access_token")?.Value;

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("No JWT token found for current user.");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<PageTransactionModel> GetTransactionDetailsAsync(TransactionInputModel model)
        {
            try
            {
                var (baseUrl, endpoint) = GetApiInfo("ApiSettings:TransactionEndpoint");

                var query = BuildQuery(
                    singles: new Dictionary<string, string?>
                    {
                        { "startDate", model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "endDate",   model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "page",      model.page.ToString() },
                        { "pageSize",  model.pageSize.ToString() }
                    },
                    multiples: BuildMultiFilters(model)
                );

                var url = $"{baseUrl}{endpoint}?{query}";
                Console.WriteLine($"[TransactionService] GET: {url}");

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddBearerToken(request);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[WARN] Transaction API returned {response.StatusCode}");
                    return new PageTransactionModel { items = new List<TransactionModel>() };
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PageTransactionModel>(json);

                data ??= new PageTransactionModel();
                data.items ??= new List<TransactionModel>();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetTransactionDetailsAsync → {ex.Message}");
                return new PageTransactionModel { items = new List<TransactionModel>() };
            }
        }

        public async Task<PageTransactionModel> GetTransactionExportAsync(TransactionInputModel model)
        {
            try
            {
                var (baseUrl, endpoint) = GetApiInfo("ApiSettings:TransactionEndpoint");

                var query = BuildQuery(
                    singles: new Dictionary<string, string?>
                    {
                        { "startDate", model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "endDate",   model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "exportAll", "true" }
                    },
                    multiples: BuildMultiFilters(model)
                );

                var url = $"{baseUrl}{endpoint}?{query}";
                Console.WriteLine($"[TransactionService] EXPORT GET: {url}");

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddBearerToken(request);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[WARN] Transaction export API returned {response.StatusCode}");
                    return new PageTransactionModel { items = new List<TransactionModel>() };
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PageTransactionModel>(json);

                data ??= new PageTransactionModel();
                data.items ??= new List<TransactionModel>();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetTransactionExportAsync → {ex.Message}");
                return new PageTransactionModel { items = new List<TransactionModel>() };
            }
        }

        public async Task<TransactionInputModel> GetTransactionFilterOptionsAsync(TransactionInputModel model)
        {
            try
            {
                var (baseUrl, endpoint) = GetApiInfo("ApiSettings:TransactionFilterOptionsEndpoint");

                var query = BuildQuery(new Dictionary<string, string?>
                {
                    { "startDate", model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "endDate",   model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss") }
                });

                var url = $"{baseUrl}{endpoint}?{query}";
                Console.WriteLine($"[TransactionService] Filter Options GET: {url}");

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddBearerToken(request);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[WARN] Filter API returned {response.StatusCode}");
                    return new TransactionInputModel();
                }

                var json = await response.Content.ReadAsStringAsync();
                var filters = JsonConvert.DeserializeObject<TransactionInputModel>(json) ?? new TransactionInputModel();

                filters.Shifts ??= new List<string>();
                filters.TollOperators ??= new List<string>();
                filters.Lanes ??= new List<string>();
                filters.PaymentMethods ??= new List<string>();

                return filters;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetTransactionFilterOptionsAsync → {ex.Message}");
                return new TransactionInputModel();
            }
        }
    }
}