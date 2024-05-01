using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace eShopSolution.AdminApp.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public UserApiService(IHttpClientFactory httpClientFactory,IConfiguration configuration) {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<ApiResult<string>> Authenticate(LoginUserRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json,Encoding.UTF8,"application/json");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["Address:Base"]);
            var response = await client.PostAsync("/api/Users/Login", httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if(response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(body);
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(body);
        }

        public async Task<ApiResult<PageResult<UserViewModel>>> GetAllUser(ViewListUserPagingRequest request)
        {
          
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["Address:Base"]);

            /*client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer " + request);*/ // them bearer token vo
            var response = await client.GetAsync($"/api/Users/GetUser?Keyword={request.Keyword}&pageIndex={request.pageIndex}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<PageResult<UserViewModel>>>(body);
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<PageResult<UserViewModel>>>(body);

        }

        public async Task<ApiResult<IdentityResult>> Register(RegisterUserRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["Address:Base"]);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Users/Register",httpContent);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<IdentityResult>>(body);
            }
            return JsonConvert.DeserializeObject<ApiErrorResult<IdentityResult>>(body);

        }
    }
}
