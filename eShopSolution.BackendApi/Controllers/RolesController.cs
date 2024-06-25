using eShopSolution.Application.System.Role;
using eShopSolution.ViewModel.System.Role;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService) 
        {
            _roleService = roleService;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index([FromQuery] ViewRolePagingRequest request) 
        {
            try
            {
                var status = await _roleService.GetAllRole(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetRolesForView")]
        public async Task<IActionResult> GetRolesForView()
        {
            try
            {
                var status = await _roleService.GetRolesForView();
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            try
            {
                var status = await _roleService.Create(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Edit")]
        public async Task<IActionResult> Edit([FromBody] EditRoleRequest request)
        {
            try
            {
                var status = await _roleService.Edit(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Detail")]
        public async Task<IActionResult> Detail([FromQuery] ViewDetailRoleRequest request)
        {
            try
            {
                var status = await _roleService.GetRoleById(request.RoleId);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery] DeleteRoleRequest request)
        {
            try
            {
                var status = await _roleService.Delete(request);
                if (status.IsSuccessed)
                {
                    return Ok(status);
                }
                return BadRequest(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
