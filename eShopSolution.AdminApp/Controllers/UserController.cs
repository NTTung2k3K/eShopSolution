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
using eShopSolution.AdminApp.Services.User;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using eShopSolution.Data.Entities;
using eShopSolution.AdminApp.Services.Role;
using eShopSolution.ViewModel.System.Role;

namespace eShopSolution.AdminApp.Controllers
{

    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Manager")]
    public class UserController : BaseController
    {
        private readonly IUserApiService _userApiService;
        private readonly IConfiguration _configuration;
        private readonly IRoleApiService _roleApiService;

        public UserController(IUserApiService userApiService, IConfiguration configuration, IRoleApiService roleApiService)
        {
            _userApiService = userApiService;
            _configuration = configuration;
            _roleApiService = roleApiService;
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            
            ViewBag.ListRole = _roleApiService.GetRolesForView().Result.ResultObj.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var status = await _userApiService.Register(request);

            ViewBag.ListRole = _roleApiService.GetRolesForView().Result.ResultObj.ToList();

            if (status is ApiErrorResult<IdentityResult> errorResult)
            {
                ViewBag.Errors = errorResult.ValidationErrors;
                return View();
            }
            TempData["SuccessMsg"] = "Create success for Username " + request.UserName;

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
                ViewBag.txtLastSeachValue = request.Keyword;
                if (!ModelState.IsValid)
                {
                    return View();
                }
                if (TempData["FailMsg"] != null)
                {
                    ViewBag.FailMsg = TempData["FailMsg"];
                }
                if (TempData["SuccessMsg"] != null)
                {
                    ViewBag.SuccessMsg = TempData["SuccessMsg"];
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
                ViewBag.ListRole = _roleApiService.GetRolesForView().Result.ResultObj.ToList();

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
                ViewBag.ListRole = _roleApiService.GetRolesForView().Result.ResultObj.ToList();


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
                TempData["SuccessMsg"] = "Edit success for Username with phoneNumber " + request.PhoneNumber;
                return RedirectToAction("GetAllUser", "User");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(ViewDetailUserRequest request)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }
                var status = await _userApiService.Detail(request);
                if (status is ApiErrorResult<UserViewModel> errorResult)
                {
                    ViewBag.Error = "Cannot get this userId " + request.UserId.ToString();
                    return View();
                }
                return View(status.ResultObj);
            }
            catch
            {
                return View();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid UserId)
        {
            try
            {

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
        public async Task<IActionResult> Delete(DeleteUserRequest request)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }
                var status = await _userApiService.Delete(request);
                if (status is ApiErrorResult<bool> errorResult)
                {
                    ViewBag.Errors = errorResult.ValidationErrors;
                    return View();
                }
                TempData["SuccessMsg"] = "Delete success UserId " + request.Id;
                return RedirectToAction("GetAllUser", "User");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }

    }


}
