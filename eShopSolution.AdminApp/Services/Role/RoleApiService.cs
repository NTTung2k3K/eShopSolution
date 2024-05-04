using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.Role;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Identity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace eShopSolution.AdminApp.Services.Role
{
    public class RoleApiService : BaseApiService, IRoleApiService
    {
        public RoleApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration, httpContextAccessor)
        {
        }
        public async Task<ApiResult<bool>> Create(CreateRoleRequest request)
        {
            var data = await PostAsync<ApiResult<bool>>($"api/Roles/Create",request);
            return data;
        }

        public async Task<ApiResult<bool>> Delete(DeleteRoleRequest request)
        {
            var data = await DeleteAsync<ApiResult<bool>>($"/api/Roles/Delete?RoleId={request.RoleId}");
            return data;
        }

        public async Task<ApiResult<bool>> Edit(EditRoleRequest request)
        {
            var data = await PutAsync<ApiResult<bool>>($"api/Roles/Edit", request);
            return data;
        }

        public async Task<ApiResult<PageResult<RoleViewModel>>> GetAllRole(ViewRolePagingRequest request)
        {
            var data =  await GetAsync<ApiResult<PageResult<RoleViewModel>>>($"api/Roles/Index?Keyword={request.Keyword}&&pageIndex={request.pageIndex}");
            return data;
        }

        public async Task<ApiResult<RoleViewModel>> GetRoleById(Guid RoleId)
        {
            var data = await GetAsync<ApiResult<RoleViewModel>>($"api/Roles/Detail?RoleId={RoleId}");
            return data;
        }
    }
}
