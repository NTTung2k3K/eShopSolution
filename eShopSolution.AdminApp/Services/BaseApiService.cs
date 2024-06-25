using eShopSolution.ViewModel.Common;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace eShopSolution.AdminApp.Services
{
    public class BaseApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async Task<TResponse> PostAsync<TResponse> (string url,Object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var httpContent = new StringContent(json,Encoding.UTF8,"application/json");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[eShopSolution.Utilities.Constants.Systemconstant.AppSettings.BaseAddress]);
            var session = _httpContextAccessor.HttpContext.Session.GetString(eShopSolution.Utilities.Constants.Systemconstant.AppSettings.Token);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            var response = await client.PostAsync(url, httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TResponse>(body);
            }
            return JsonConvert.DeserializeObject<TResponse>(body);
        }
        protected async Task<TResponse> GetAsync<TResponse>(string url)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString(eShopSolution.Utilities.Constants.Systemconstant.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[eShopSolution.Utilities.Constants.Systemconstant.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TResponse>(body);
            }
            return JsonConvert.DeserializeObject<TResponse>(body);
        }
        protected async Task<TResponse> PutAsync<TResponse>(string url,Object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var session = _httpContextAccessor.HttpContext.Session.GetString(eShopSolution.Utilities.Constants.Systemconstant.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[eShopSolution.Utilities.Constants.Systemconstant.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            var response = await client.PutAsync(url, httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TResponse>(body);
            }
            return JsonConvert.DeserializeObject<TResponse>(body);
        }

        protected async Task<TResponse> DeleteAsync<TResponse>(string url)
        {
            var session = _httpContextAccessor.HttpContext.Session.GetString(eShopSolution.Utilities.Constants.Systemconstant.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[eShopSolution.Utilities.Constants.Systemconstant.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            var response = await client.DeleteAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TResponse>(body);
            }
            return JsonConvert.DeserializeObject<TResponse>(body);
        }

        protected async Task<TResponse> PatchAsync<TResponse>(string url, Object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var session = _httpContextAccessor.HttpContext.Session.GetString(eShopSolution.Utilities.Constants.Systemconstant.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[eShopSolution.Utilities.Constants.Systemconstant.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
            var response = await client.PatchAsync(url, httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TResponse>(body);
            }
            return JsonConvert.DeserializeObject<TResponse>(body);
        }


    }
}
