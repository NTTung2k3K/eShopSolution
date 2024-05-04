using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eShopSolution.AdminApp.Services.User;

namespace eShopSolution.AdminApp.Controllers
{
    public class LoginController : Controller
    {

        private readonly IUserApiService _userApiService;
        private readonly IConfiguration _configuration;

        public LoginController(IUserApiService userApiService, IConfiguration configuration)
        {
            _userApiService = userApiService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Index(LoginUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Incorrect Email or password";
                return View();
            }
            var apiResult = await _userApiService.Authenticate(request);

            if (apiResult.IsSuccessed)
            {
                var userPrincipal = this.ValidateToken(apiResult.ResultObj);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    IsPersistent = false
                };
                HttpContext.Session.SetString("token", apiResult.ResultObj);
                await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            userPrincipal,
                            authProperties);

                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Incorrect Email or password";
            return View();
        }

        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["Jwt:Audience"];
            validationParameters.ValidIssuer = _configuration["Jwt:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }
    }
}
