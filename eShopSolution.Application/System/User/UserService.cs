using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModel.Catalog.Common;
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

        public UserService(EShopDBContext context ,UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuarion = configuration;
            _signInManager = signInManager;
            _dbText = context;
        }

        public async Task<PageResult<UserViewModel>> GetListUser(ViewListUserPagingRequest request)
        {
            var listUser = _dbText.Users.AsQueryable();
            if(request.Keyword != null) {
             listUser = listUser.Where(x => x.UserName.Contains(request.Keyword) 
             || x.Email.Contains(request.Keyword) || x.PhoneNumber.Contains(request.Keyword));
            }
            listUser = listUser.OrderByDescending(x => x.UserName);
            var listPaging = listUser.ToPagedList(request.pageIndex, request.pageSize).ToList();



            var listResult = new List<UserViewModel>();
            foreach (var item in listPaging)
            {
                var user = new UserViewModel()
                {
                    Username = item.UserName,
                    PhoneNumber = item.PhoneNumber,
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    Dob = item.Dob,
                };
                var appUser = await _userManager.FindByIdAsync(item.Id.ToString());
                var roles = await _userManager.GetRolesAsync(appUser);
                if(roles != null)
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
                TotalCount = listUser.Count()
            };
            return result;
        }

        public async Task<string> Login(LoginUserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            var userConfirm = await _userManager.CheckPasswordAsync(user, request.Password);
            if (user == null || userConfirm == false)
            {
                return "";
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
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<IdentityResult> Register(RegisterUserRequest request)
        {
            if (!request.Password.Equals(request.ConfirmPassword))
            {
                throw new Exception("Not same password");
            }
            var userFindByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userFindByEmail != null)
            {
                throw new Exception("Email is existed");

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
            if (status.Succeeded)
            {
                bool statusRole = await _roleManager.RoleExistsAsync(eShopSolution.ViewModel.Catalog.Users.UserRole.ADMIN);
                if (!statusRole)
                {
                    var role = new AppRole()
                    {
                        Name = eShopSolution.ViewModel.Catalog.Users.UserRole.ADMIN,
                        Description = "Administrator role"
                    };
                    var result = await _roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Erro to create role, please contact with manager");
                    }

                }
                await _userManager.AddToRoleAsync(user, eShopSolution.ViewModel.Catalog.Users.UserRole.ADMIN);

            }
            return status;
        }
    }
}
