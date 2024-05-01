using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.User
{
    public interface IUserService
    {
        public Task<ApiResult<string>> Login(LoginUserRequest request);
        public Task<ApiResult<IdentityResult>> Register(RegisterUserRequest request);
        public Task<ApiResult<PageResult<UserViewModel>>> GetListUser(ViewListUserPagingRequest request);
        
        public Task<ApiResult<bool>> Edit(EditUserRequest request);
        public Task<ApiResult<UserViewModel>> GetUserById(Guid id);



    }
}
