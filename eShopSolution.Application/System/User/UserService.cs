using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModel.Catalog.Common;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PagedList;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuarion;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EShopDBContext _dbText;

        public UserService(EShopDBContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuarion = configuration;
            _signInManager = signInManager;
            _dbText = context;
        }

        public async Task<ApiResult<bool>> Delete(DeleteUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Not Found");
            }
            var status = await _userManager.DeleteAsync(user);
            if (!status.Succeeded)
            {
                return new ApiErrorResult<bool>("Fail");
            }
            return new ApiSuccessResult<bool>("Success");

        }

        public async Task<ApiResult<UserViewModel>> Detail(ViewDetailUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserViewModel>("Not Found");
            }
            var userVm = new UserViewModel()
            {
                Id = user.Id,
                Dob = user.Dob,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName
            };
            var listRoleOfUser = await _userManager.GetRolesAsync(user);

            if (listRoleOfUser.Count > 0)
            {
                userVm.Roles = new List<string>();
                foreach (var role in listRoleOfUser)
                {
                    userVm.Roles.Add(role);
                }
            }
            return new ApiSuccessResult<UserViewModel>(userVm, "Success");
        }

        public async Task<ApiResult<bool>> Edit( EditUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());



            if(user == null)
            {
                return new  ApiErrorResult<bool>("Not Found");
            }
            user.PhoneNumber= request.PhoneNumber;
            user.FirstName= request.FirstName;
            user.LastName= request.LastName;
            user.Dob= request.Dob;
            if(request.Roles.Count > 0)
            {
                

                var listRole = await _userManager.GetRolesAsync(user);
                foreach (var role in listRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
                foreach (var roleAdd in request.Roles)
                {
                    var role = await _roleManager.FindByIdAsync(roleAdd.ToString());
                    if (role == null) return new ApiErrorResult<bool>("Role is not exited!");
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
            }
            else
            {
                var listRole = await _userManager.GetRolesAsync(user);
                foreach (var role in listRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }
            var statusUser = await _userManager.UpdateAsync(user);
            if(statusUser.Succeeded)
            {
                return new ApiSuccessResult<bool>("Success");
            }
            return new ApiErrorResult<bool>("Fail");

        }

        public async Task<ApiResult<PageResult<UserViewModel>>> GetListUser(ViewListUserPagingRequest request)
        {
            var listUser = _dbText.Users.AsQueryable();
            if (request.Keyword != null)
            {
                listUser = listUser.Where(x => x.UserName.Contains(request.Keyword)
                || x.Email.Contains(request.Keyword) || x.PhoneNumber.Contains(request.Keyword));
            }
            listUser = listUser.OrderByDescending(x => x.UserName);
            int pageIndex = request.pageIndex ?? 1;

            var listPaging = listUser.ToPagedList(pageIndex, ViewModel.Common.PageInfo.PAGE_SIZE).ToList();



            var listResult = new List<UserViewModel>();
            foreach (var item in listPaging)
            {
                var user = new UserViewModel()
                {
                    Id = item.Id,
                    Username = item.UserName,
                    PhoneNumber = item.PhoneNumber,
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    Dob = item.Dob,
                };
                var appUser = await _userManager.FindByIdAsync(item.Id.ToString());
                var roles = await _userManager.GetRolesAsync(appUser);
                if (roles != null)
                {
                    user.Roles = new List<string>();
                    foreach (var role in roles)
                    {
                        user.Roles.Add(role);
                    }
                    listResult.Add(user);
                }

            }

            var result = new PageResult<UserViewModel>()
            {
                Items = listResult,
                TotalRecords = listUser.Count(),
                PageIndex = pageIndex,
                PageSize = ViewModel.Common.PageInfo.PAGE_SIZE
            };
            var apiSuccess = new ApiSuccessResult<PageResult<UserViewModel>>(result, "Success");
            return apiSuccess;
        }

        public async Task<ApiResult<UserViewModel>> GetUserById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserViewModel>("Not Found");
            }
            var userVm = new UserViewModel()
            {
                Id = user.Id,
                Dob = user.Dob,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName
            };
            var listRoleOfUser = await _userManager.GetRolesAsync(user);

            if (listRoleOfUser.Count > 0)
            {
                userVm.Roles = new List<string>();
                foreach (var role in listRoleOfUser)
                {
                    userVm.Roles.Add(role);
                }
            }
            return new ApiSuccessResult<UserViewModel>(userVm, "Success");
        }

        public async Task<ApiResult<string>> Login(LoginUserRequest request) // hàm
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                var errorApi = new ApiErrorResult<string>("Invalid Email or Password");
                return errorApi;
            }


            var user = await _userManager.FindByEmailAsync(request.Email);
            var userConfirm = await _userManager.CheckPasswordAsync(user, request.Password);
            if (user == null || userConfirm == false)
            {
                var errorApi = new ApiErrorResult<string>("Invalid Email or Password");
                return errorApi;
            }
            var authClaim = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.FirstName+" "+user.LastName),
                new Claim(ClaimTypes.Email,user.Email),
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var item in roles)
            {
                authClaim.Add(new Claim(ClaimTypes.Role, item.ToString()));
            }
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuarion["Jwt:Key"]));
            var token = new JwtSecurityToken(
                    issuer: _configuarion["Jwt:Issuer"],
                    audience: _configuarion["Jwt:Audience"],
                    claims: authClaim,
                    expires: DateTime.Now.AddMinutes(3),
                    signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha512Signature)
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var ApiSuccess = new ApiSuccessResult<string>(tokenString, "Success");

            return ApiSuccess;

        }

        public async Task<ApiResult<IdentityResult>> Register(RegisterUserRequest request)
        {
            if (!request.Password.Equals(request.ConfirmPassword))
            {
                var errorApi = new ApiErrorResult<IdentityResult>("Not same password");
                return errorApi;
            }
            var userFindByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userFindByEmail != null)
            {
                var errorApi = new ApiErrorResult<IdentityResult>("Email is existed");
                return errorApi;

            }

            var user = new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                Dob = request.Dob,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };


            var status = await _userManager.CreateAsync(user, request.Password);
            if (!status.Succeeded)
            {
                var errorApi = new ApiErrorResult<IdentityResult>("Error");
                errorApi.ValidationErrors = new List<string>();
                foreach (var item in status.Errors)
                {
                    errorApi.ValidationErrors.Add(item.Description);
                }
                return errorApi;
            }


/*
            foreach (var roleId in request.Roles)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role == null) return new ApiErrorResult<IdentityResult>("Role is not exited!");
                await _userManager.AddToRoleAsync(user, role.Name);
            }*/
            var ApiSuccess = new ApiSuccessResult<IdentityResult>("Successed");
            return ApiSuccess;
        }
    }
}
