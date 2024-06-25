using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.Role;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Role
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuarion;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EShopDBContext _dbText;

        public RoleService(EShopDBContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuarion = configuration;
            _signInManager = signInManager;
            _dbText = context;
        }

        public async Task<ApiResult<bool>> Create(CreateRoleRequest request)
        {
            var role = await _roleManager.FindByNameAsync(request.Name);
            if (role != null)
            {
                return new ApiErrorResult<bool>("Role existed");
            }
            var roleAdd = new AppRole()
            {
                Name = request.Name,
                Description = request.Description
            };
            var status = await _roleManager.CreateAsync(roleAdd);
            if (!status.Succeeded)
            {
                return new ApiErrorResult<bool>("System error");
            }
            return new ApiSuccessResult<bool>("Success");

        }

        public async Task<ApiResult<bool>> Delete(DeleteRoleRequest request)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
            if (role == null)
            {
                return new ApiErrorResult<bool>("Not found");
            }
            var status = await _roleManager.DeleteAsync(role);
            if (!status.Succeeded)
            {
                return new ApiErrorResult<bool>("System error");
            }
            return new ApiSuccessResult<bool>("Success");
        }

        public async Task<ApiResult<RoleViewModel>> GetRoleById(Guid RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId.ToString());
            if (role == null)
            {
                return new ApiErrorResult<RoleViewModel>("Not found");
            }
            var RoleVm = new RoleViewModel()
            {
                RoleId = role.Id,
                Description = role.Description,
                Name = role.Name
            };
            return new ApiSuccessResult<RoleViewModel>(RoleVm, "Success");
        }

        public async Task<ApiResult<bool>> Edit(EditRoleRequest request)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
            if (role == null)
            {
                return new ApiErrorResult<bool>("Not found");
            }
            role.Name = request.Name;
            role.Description = request.Description;
            var status = await _roleManager.UpdateAsync(role);
            if (!status.Succeeded)
            {
                return new ApiErrorResult<bool>("System error");
            }
            return new ApiSuccessResult<bool>("Success");
        }

        public async Task<ApiResult<PageResult<RoleViewModel>>> GetAllRole(ViewRolePagingRequest request)
        {
            var listRole = await _roleManager.Roles.ToListAsync();
            if (request.Keyword != null)
            {
                listRole = listRole.Where(x => x.Name.Contains(request.Keyword) ||
                x.Description.Contains(request.Keyword)).ToList();
            }
            listRole = listRole.OrderBy(x => x.Name).ToList();

            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listRole.ToPagedList(pageIndex, eShopSolution.ViewModel.Common.PageInfo.PAGE_SIZE).ToList();

            var listRoleViewModel = listPaging.Select(x => new RoleViewModel()
            {
                RoleId = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
            var listResult = new PageResult<RoleViewModel>()
            {
                Items = listRoleViewModel,
                PageSize = eShopSolution.ViewModel.Common.PageInfo.PAGE_SIZE,
                TotalRecords = listRole.Count,
                PageIndex = pageIndex
            };
            return new ApiSuccessResult<PageResult<RoleViewModel>>(listResult, "Success");
        }

        public async Task<ApiResult<List<RoleViewModel>>> GetRolesForView()
        {
            var listRole = await _roleManager.Roles.ToListAsync();
            var listRoleViewModel = listRole.Select(x => new RoleViewModel()
            {
                RoleId = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
            return new ApiSuccessResult<List<RoleViewModel>>(listRoleViewModel,"Success");

        }
    }
}
