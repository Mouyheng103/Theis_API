using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static API.Data.serviceResponses;

namespace API.Controllers.Users
{

    [Route("api/user/")]
    [ApiController]
    //[Authorize]
    public class RoleController(RoleManager<Roles> roleManager, IConfiguration config) : ControllerBase
    {
        [HttpGet("getrole")]
        public IActionResult GetRole()
        {
            var role = roleManager.Roles.ToList();
            return Ok(role);
        }
        [HttpGet("findrole/{roleName}")]
        public async Task<IActionResult> FindRole(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound(new { error = "Role not found" });
            }
            return Ok(role);
        }
        [HttpPost("addrole")]
        //[Authorize(Roles = "Admin")]
        public async Task<GeneralResponse> AddRole(RoleDTO roleDTO)
        {
            if (roleDTO == null) return new GeneralResponse(false, "Model is Empty");
            var newRole = new Roles()
            {
                Name = roleDTO.Name,
                Description = roleDTO.Description
            };
            var role= await roleManager.FindByNameAsync(newRole.Name);
            if (role is not null) return new GeneralResponse(false, "Role Already Exsist");
            var addRole= await roleManager.CreateAsync(newRole);
            if (!addRole.Succeeded)
            {
                var errors = string.Join(", ", addRole.Errors.Select(e => e.Description));
                return new GeneralResponse(false, $"Error occurred: {errors}");
            }
            return new GeneralResponse(true, $"Role {roleDTO.Name} Has Been Created");
        }
        [HttpPut("updaterole/{roleName}")]
        public async Task<GeneralResponse> UpdateRole(string roleName,RoleDTO roleDTO)
        {
            var findrole = await roleManager.FindByNameAsync(roleName);
            if (findrole is null) return new GeneralResponse(false, "Role Not Found");
            findrole.Name = roleDTO.Name;
            findrole.Description = roleDTO.Description;
            
            var updateRole= await roleManager.UpdateAsync(findrole);
            if (!updateRole.Succeeded)
            {
                var errors = string.Join(", ", updateRole.Errors.Select(e => e.Description).Distinct());
                return new GeneralResponse(false, $"Error occurred: {errors}");
            }
            return new GeneralResponse(true, $"Role {roleName} updated successfully to {roleDTO.Name} with description {roleDTO.Description}");
        }
        [HttpDelete("deleterole/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var findrole = await roleManager.FindByNameAsync(roleName);
            if (findrole is null) return NotFound(new { error = "Role not found" });
            var deleteRole= await roleManager.DeleteAsync(findrole);
            if (deleteRole.Succeeded)
            {
                return Ok(new { result = $"Role {roleName} deleted successfully" });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Role deletion failed" });
        }

    }
}
