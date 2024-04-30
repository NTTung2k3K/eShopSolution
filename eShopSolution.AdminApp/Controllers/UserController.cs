using eShopSolution.AdminApp.Services;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eShopSolution.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace eShopSolution.AdminApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserApiService _userApiService;
        private readonly IConfiguration _configuration;

        public UserController(IUserApiService userApiService,IConfiguration configuration) {
            _userApiService = userApiService;
            _configuration = configuration;
        }

       

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("token");
            return RedirectToAction("Index", "Login");
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUser(ViewListUserPagingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                var listUser = await _userApiService.GetAllUser(request);
                return View(listUser);
            }
            catch
            {
                return View();
            }
           
        }
    }
}
