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
using eShopSolution.ViewModel.Common;
using Microsoft.AspNetCore.Identity;
using eShopSolution.Data.Enums;

namespace eShopSolution.AdminApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserApiService _userApiService;
        private readonly IConfiguration _configuration;

        public UserController(IUserApiService userApiService, IConfiguration configuration)
        {
            _userApiService = userApiService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {

            ViewBag.ListRole = await ViewModel.Catalog.Users.UserRole.getListRoleAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var status = await _userApiService.Register(request);
            ViewBag.ListRole = await ViewModel.Catalog.Users.UserRole.getListRoleAsync();
            if (status is ApiErrorResult<IdentityResult> errorResult)
            {
                ViewBag.Errors = errorResult.ValidationErrors;
                return View();
            }

            return RedirectToAction("GetAllUser", "User");
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
                return View(listUser.ResultObj);
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid UserId)
        {
            try
            {
                ViewBag.ListRole = await ViewModel.Catalog.Users.UserRole.getListRoleAsync();

                if (!ModelState.IsValid)
                {
                    return View();
                }

                var User = await _userApiService.GetUserById(UserId);
                return View(User.ResultObj);
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserRequest request)
        {
            try
            {
                ViewBag.ListRole = await ViewModel.Catalog.Users.UserRole.getListRoleAsync();

                if (!ModelState.IsValid)
                {
                    return View();
                }
                var status = await _userApiService.Edit(request);
                if (status is ApiErrorResult<bool> errorResult)
                {
                    ViewBag.Errors = errorResult.ValidationErrors;
                    return View();
                }

                return RedirectToAction("GetAllUser", "User");
            }
            catch
            {
                return View();
            }
        }


    }
}
