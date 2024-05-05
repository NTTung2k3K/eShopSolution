using eShopSolution.AdminApp.Services.Role;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.Role;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.AdminApp.Controllers
{

    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Manager")]
    public class RoleController : Controller
    {
        private readonly IRoleApiService _roleApiService;

        public RoleController(IRoleApiService roleApiService) 
        { 
            _roleApiService = roleApiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(ViewRolePagingRequest request)
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

                var User = await _roleApiService.GetAllRole(request);
                return View(User.ResultObj);
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Detail(ViewDetailRoleRequest request)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }
                var status = await _roleApiService.GetRoleById(request.RoleId);
                if (status is ApiErrorResult<RoleViewModel> errorResult)
                {
                    ViewBag.Error = "Cannot get this userId " + request.RoleId.ToString();
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
        public async Task<IActionResult> Edit(Guid RoleId)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                var User = await _roleApiService.GetRoleById(RoleId);
                return View(User.ResultObj);
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleRequest request)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }
                var status = await _roleApiService.Edit(request);
                if (status is ApiErrorResult<bool> errorResult)
                {
                    ViewBag.Errors = errorResult.ValidationErrors;
                    return View();
                }
                TempData["SuccessMsg"] = "Edit success for Role " + request.RoleId;
                return RedirectToAction("Index", "Role");
            }
            catch
            {
                return View();
            }
        }



        [HttpGet]
        public async Task<IActionResult> Delete(Guid RoleId)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                var User = await _roleApiService.GetRoleById(RoleId);
                return View(User.ResultObj);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteRoleRequest request)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }
                var status = await _roleApiService.Delete(request);
                if (status is ApiErrorResult<bool> errorResult)
                {
                    ViewBag.Errors = errorResult.ValidationErrors;
                    return View();
                }
                TempData["SuccessMsg"] = "Delete success RoleId " + request.RoleId;
                return RedirectToAction("Index", "Role");
            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleRequest request)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            var status = await _roleApiService.Create(request);

            if (status is ApiErrorResult<bool> errorResult)
            {
                ViewBag.Errors = errorResult.Message;
                return View();
            }
            TempData["SuccessMsg"] = "Create success for Role " + request.Name;

            return RedirectToAction("Index", "Role");
        }


    }
}
