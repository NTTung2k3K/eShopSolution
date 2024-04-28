using eShopSolution.AdminApp.Services;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.AdminApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserApiService _userApiService;

        public UserController(IUserApiService userApiService) {
            _userApiService = userApiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }
            var token = await _userApiService.Authenticate(request);
            return View(token);
        }
    }
}
