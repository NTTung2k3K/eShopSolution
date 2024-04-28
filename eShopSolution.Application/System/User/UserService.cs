using eShopSolution.Data.Entities;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

        public UserService(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager,SignInManager<AppUser> signInManager,
            IConfiguration configuration) {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuarion = configuration;
            _signInManager = signInManager;
        }
        public async Task<string> Login(LoginUserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            var userConfirm = await _userManager.CheckPasswordAsync(user, request.Password);
            if (user == null || userConfirm==false )
            {
                return "Invalid Username or Password";
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
            if(userFindByEmail != null)
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
            var status = await _userManager.CreateAsync(user,request.Password);
            if (status.Succeeded)
            {
                if(!await _roleManager.RoleExistsAsync(eShopSolution.ViewModel.Catalog.Users.UserRole.CUSTOMER))
                {
                    await _roleManager.CreateAsync(new AppRole()
                    {
                        Name = eShopSolution.ViewModel.Catalog.Users.UserRole.CUSTOMER
                    });
                }
                await _userManager.AddToRoleAsync(user, eShopSolution.ViewModel.Catalog.Users.UserRole.CUSTOMER);
            }
            return status;
        }
    }
}
