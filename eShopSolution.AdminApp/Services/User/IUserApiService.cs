using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Identity;

namespace eShopSolution.AdminApp.Services.User
{
    public interface IUserApiService
    {
        Task<ApiResult<string>> Authenticate(LoginUserRequest request);

        Task<ApiResult<PageResult<UserViewModel>>> GetAllUser(ViewListUserPagingRequest request);
        Task<ApiResult<IdentityResult>> Register(RegisterUserRequest request);

        Task<ApiResult<bool>> Edit(EditUserRequest request);
        Task<ApiResult<UserViewModel>> GetUserById(Guid id);

        Task<ApiResult<UserViewModel>> Detail(ViewDetailUserRequest request);
        Task<ApiResult<bool>> Delete(DeleteUserRequest request);
    }
}
