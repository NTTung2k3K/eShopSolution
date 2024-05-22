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
using Microsoft.AspNetCore.Identity;
using eShopSolution.Data.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Owin.Security;
using NuGet.Protocol.Plugins;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Google;
using System.Web;

namespace eShopSolution.AdminApp.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {

        private readonly IUserApiService _userApiService;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public LoginController(IMapper mapper ,IUserApiService userApiService, IConfiguration configuration, SignInManager<AppUser> sigInManager, UserManager<AppUser> userManager)
        {
            _userApiService = userApiService;
            _configuration = configuration;
            _signInManager = sigInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Inzdex(LoginUserRequest request)
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
                var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
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

        [AllowAnonymous]
        public IActionResult GoogleLogin(string provider)
        {
            string redirectUrl = Url.Action("GoogleLoginResponse", "Login");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            var result = new ChallengeResult(provider, properties);
            return result;
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleLoginResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("LoginPortal");
            }
            else
            {
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AppUser user = null;
                    if (info.LoginProvider == "Facebook")
                    {
                        Random rd = new Random();
                        string prefix = "FB"+ rd.Next(0,9).ToString() + rd.Next(0,9).ToString() + rd.Next(0,9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() ;
                        string username = prefix;
                         user = new AppUser()
                        {
                            FirstName = info.Principal.FindFirst(ClaimTypes.GivenName)?.Value,
                            LastName = info.Principal.FindFirst(ClaimTypes.Surname)?.Value, // Example: Surname claim
                            Email = info.Principal.FindFirst(ClaimTypes.Email)?.Value,
                            UserName = username,
                        };
                    }

                    if (info.LoginProvider == "Google")
                    {
                        Random rd = new Random();
                        string prefix = "GG" + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString() + rd.Next(0, 9).ToString();
                        string username = prefix;


                        user = new AppUser()
                        {
                            FirstName = info.Principal.FindFirst(ClaimTypes.GivenName)?.Value,
                            LastName = info.Principal.FindFirst(ClaimTypes.Surname)?.Value, // Example: Surname claim
                            Email = info.Principal.FindFirst(ClaimTypes.Email)?.Value,
                            UserName = username,
                        };
                    }

                   
                    
                    var createUserResult = await _userManager.CreateAsync(user);

                    if (createUserResult.Succeeded)
                    {
                        var identResult = await _userManager.AddLoginAsync(user, info);
                        if (identResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, false);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            return View();
        }



    }
}
