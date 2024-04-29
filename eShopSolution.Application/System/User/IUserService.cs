using eShopSolution.ViewModel.Catalog.Common;
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
        public Task<string> Login(LoginUserRequest request);
        public Task<IdentityResult> Register(RegisterUserRequest request);

        public Task<PageResult<UserViewModel>> GetListUser(ViewListUserPagingRequest request);
    }
}
