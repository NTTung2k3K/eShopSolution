using eShopSolution.Application.System.User;
using eShopSolution.Utilities;
using eShopSolution.ViewModel.System.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            try
            {
                string token = await _userService.Login(request);
                return Ok(token);
            }
            catch (Exception ex)
            {
                var e = new eShopException(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            try
            {
                var status = await _userService.Register(request);
                if (status.Errors.Count() > 0)
                {
                    return Ok(status.Errors);
                }
                else
                {
                    return Ok("Created");
                }
            }
            catch (Exception ex)
            {
                var e = new eShopException(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
