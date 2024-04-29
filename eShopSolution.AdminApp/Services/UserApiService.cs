using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.System.User;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

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
        public async Task<string> Authenticate(LoginUserRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json,Encoding.UTF8,"application/json");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["Address:Base"]);
            var response = await client.PostAsync("/api/Users/Login", httpContent);
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }

        public async Task<PageResult<UserViewModel>> GetAllUser(ViewListUserPagingRequest request)
        {
          
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["Address:Base"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer " + request); // them bearer token vo
            var response = await client.GetAsync($"/api/User/GetUser?Keyword={request}&pageIndex={request.pageIndex}&pageSize={request.pageSize}");
            var body = await response.Content.ReadAsStringAsync();
            var listUser = JsonConvert.DeserializeObject<PageResult<UserViewModel>>(body);
            return listUser;

        }
    }
}
