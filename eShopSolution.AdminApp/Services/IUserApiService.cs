using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Identity;

namespace eShopSolution.AdminApp.Services
{
    public interface IUserApiService
    {
        Task<ApiResult<string>> Authenticate(LoginUserRequest request);

        Task<ApiResult<PageResult<UserViewModel>>> GetAllUser(ViewListUserPagingRequest request);
        Task<ApiResult<IdentityResult>> Register(RegisterUserRequest request);
    }
}
