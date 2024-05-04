using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.Role;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Role
{
    public interface IRoleService
    {
        public Task<ApiResult<PageResult<RoleViewModel>>> GetAllRole(ViewRolePagingRequest request);
        public Task<ApiResult<bool>> Create(CreateRoleRequest request);
        public Task<ApiResult<bool>> Delete(DeleteRoleRequest request);
        public Task<ApiResult<bool>> Edit(EditRoleRequest request);
        public Task<ApiResult<RoleViewModel>> GetRoleById(Guid RoleId);


    }
}
