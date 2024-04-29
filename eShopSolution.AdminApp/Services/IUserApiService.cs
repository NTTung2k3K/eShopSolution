using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.System.User;

namespace eShopSolution.AdminApp.Services
{
    public interface IUserApiService
    {
        Task<string> Authenticate(LoginUserRequest request);

        Task<PageResult<UserViewModel>> GetAllUser(ViewListUserPagingRequest request);

    }
}
