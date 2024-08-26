using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;

namespace API.Controllers.Users
{

    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<Roles> _roleManager;
        public RoleController(RoleManager<Roles> roleManager,DataContext dataContext)
        {
            _dataContext = dataContext;
            _roleManager=roleManager;
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Retrive all roles", Description = "")]
        public IActionResult GetRole()
        {
            var role = _roleManager.Roles.ToList();
            return Ok(role);
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrive a single role", Description = "")]
        public async Task<IActionResult> FindRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { error = "Role not found" });
            }
            return Ok(role);
        }
        [HttpPost]
        [SwaggerOperation(Summary = "add a single role", Description = "")]
        public async Task<IActionResult> AddRole(RoleDTO roleDTO)
        {
            if (roleDTO == null) return BadRequest(new { Message = "Model is Empty" });

            var newRole = new Roles()
            {
                Name = roleDTO.Name,
                Description = roleDTO.Description
            };

            var role = await _roleManager.FindByNameAsync(newRole.Name);
            if (role is not null) return BadRequest(new { Message = "Role Already Exists" });

            var addRole = await _roleManager.CreateAsync(newRole);
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

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "update a single role", Description = "")]
        public async Task<IActionResult> UpdateRole(string id, RoleDTO roleDTO)
        {
            var findRole = await _roleManager.FindByIdAsync(id);
            if (findRole is null) return BadRequest(new { Message = "Role Not Found" });
            var findup=_dataContext.AspNetRoles.Where(x=>x.Name == roleDTO.Name).FirstOrDefault();
            if(findup.Id !=id && findup != null)
            {
                return BadRequest(new { Message = "Role Already Exists" });
            }
            findRole.Name = roleDTO.Name;
            findRole.Description = roleDTO.Description;

            var updateRole = await _roleManager.UpdateAsync(findRole);
            if (!updateRole.Succeeded)
            {
                var errors = string.Join(", ", updateRole.Errors.Select(e => e.Description).Distinct());
                return BadRequest(new { Message = $"Error occurred: {errors}" });
            }
            return Ok(new { Message = $"Success!", Data = findRole });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "delete a signle role from db", Description = "")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var findrole = await _roleManager.FindByIdAsync(id);
            if (findrole is null) return NotFound(new { error = "Role not found" });
            var deleteRole= await _roleManager.DeleteAsync(findrole);
            if (deleteRole.Succeeded)
            {
                return Ok(new { result = $"Role deleted successfully" });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Role deletion failed" });
        }

    }
}
