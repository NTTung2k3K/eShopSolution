using eShopSolution.ViewModel.System.User;
using Newtonsoft.Json;
using System.Text;

namespace eShopSolution.AdminApp.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserApiService(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> Authenticate(LoginUserRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json,Encoding.UTF8,"application/json");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7073");
            var response = await client.PostAsync("/api/Users/Login", httpContent);
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }
    }
}
