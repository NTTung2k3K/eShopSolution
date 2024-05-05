using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.Role;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Identity;

namespace eShopSolution.AdminApp.Services.Role
{
    public interface IRoleApiService
    {
        public Task<ApiResult<PageResult<RoleViewModel>>> GetAllRole(ViewRolePagingRequest request);
        public Task<ApiResult<bool>> Create(CreateRoleRequest request);
        public Task<ApiResult<bool>> Delete(DeleteRoleRequest request);
        public Task<ApiResult<bool>> Edit(EditRoleRequest request);
        public Task<ApiResult<RoleViewModel>> GetRoleById(Guid RoleId);
        public Task<ApiResult<List<RoleViewModel>>> GetRolesForView();


    }
}
