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
    public class RoleController(RoleManager<Roles> roleManager) : ControllerBase
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
        public async Task<IActionResult> AddRole(RoleDTO roleDTO)
        {
            if (roleDTO == null) return BadRequest(new { Message = "Model is Empty" });

            var newRole = new Roles()
            {
                Name = roleDTO.Name,
                Description = roleDTO.Description
            };

            var role = await roleManager.FindByNameAsync(newRole.Name);
            if (role is not null) return BadRequest(new { Message = "Role Already Exists" });

            var addRole = await roleManager.CreateAsync(newRole);
            if (!addRole.Succeeded)
            {
                var errors = string.Join(", ", addRole.Errors.Select(e => e.Description));
                return BadRequest(new { Message = $"Error occurred: {errors}" });
            }

            var createdRoleData = new
            {
                newRole.Id,
                newRole.Name,
                newRole.Description
            };

            return Ok(new { Message = $"Role {roleDTO.Name} Has Been Created", Data = createdRoleData });
        }

        [HttpPut("updaterole/{roleName}")]
        public async Task<IActionResult> UpdateRole(string roleName, RoleDTO roleDTO)
        {
            var findRole = await roleManager.FindByNameAsync(roleName);
            if (findRole is null) return BadRequest(new { Message = "Role Not Found" });

            findRole.Name = roleDTO.Name;
            findRole.Description = roleDTO.Description;

            var updateRole = await roleManager.UpdateAsync(findRole);
            if (!updateRole.Succeeded)
            {
                var errors = string.Join(", ", updateRole.Errors.Select(e => e.Description).Distinct());
                return BadRequest(new { Message = $"Error occurred: {errors}" });
            }

            var updatedRoleData = new
            {
                findRole.Id,
                findRole.Name,
                findRole.Description
            };

            return Ok(new { Message = $"Role {roleName} updated successfully to {roleDTO.Name} with description {roleDTO.Description}", Data = updatedRoleData });
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
