using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.User
{
    [Route("api/user")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {

        private readonly UserManager<Data.Users> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        private readonly IConfiguration _config;
        private readonly DataContext _dataContext;

        public UserController(UserManager<Data.Users> userManager, RoleManager<Roles> roleManager, IConfiguration config, DataContext dataContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _dataContext = dataContext;
        }
        private IActionResult HandleException(Exception ex, string customMessage)
        {
            if (ex is SqlException)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"{customMessage} Error: {ex.Message}" });
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Retrive all users", Description = "")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var result = from user in _dataContext.Users
                             join userRole in _dataContext.UserRoles on user.Id equals userRole.UserId
                             join role in _dataContext.Roles on userRole.RoleId equals role.Id
                             join branch in _dataContext.tblO_Branch on user.BranchId equals branch.Id
                             join manager in _dataContext.tblO_Staff on branch.BranchMangerId equals manager.Id
                             select new
                             {
                                 UserId = user.Id,
                                 UserName = user.UserName,
                                 Email = user.Email,
                                 Branch = new { branch=branch,manager=manager},
                                 Role = role,
                                 AllowReset = user.AllowResetPassword,
                                 Active = user.Active,
                                 Created_At = user.Created_At,
                                 Created_by = user.Created_By,
                             };
                var dataList = await result.ToListAsync();
                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No user found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrive data to the database.");
            }
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrive a single user", Description = "")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var result = from user in _dataContext.Users where(user.Id==id) 
                             join userRole in _dataContext.UserRoles on user.Id equals userRole.UserId
                             join role in _dataContext.Roles on userRole.RoleId equals role.Id
                             join branch in _dataContext.tblO_Branch on user.BranchId equals branch.Id
                             join manager in _dataContext.tblO_Staff on branch.BranchMangerId equals manager.Id
                             select new
                             {
                                 UserId = user.Id,
                                 UserName = user.UserName,
                                 Email = user.Email,
                                 Branch = new { branch = branch, manager = manager },
                                 Role = role,
                                 AllowReset = user.AllowResetPassword,
                                 Active = user.Active,
                                 Created_At = user.Created_At,
                                 Created_by = user.Created_By,
                             };
                var dataList = await result.ToListAsync();
                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No user found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrive data to the database.");
            }
        }
        [HttpPost]
        [SwaggerOperation(Summary = "Add a single user", Description = "")]
        public async Task<IActionResult> CreateAccount(UserDTO userDTO)
        {
            try
            {
                if (userDTO is null) return BadRequest(new { Message = "Model is empty" });
                var newUser = new Data.Users()
                {
                    UserName = userDTO.UserName,
                    Email = userDTO.Email,
                    PasswordHash = userDTO.Password,
                    BranchId = userDTO.BranchId,
                    AllowResetPassword = false,
                    Active = true,
                    Created_At = DateTime.UtcNow,
                    Created_By = userDTO.Created_By

                };
                var checkdup = await _dataContext.AspNetUsers.Where(u => u.UserName == userDTO.UserName).ToListAsync();
                if (checkdup.Count > 0) return BadRequest(new { Message = "User registered already" });
                try
                {
                    var role = await _roleManager.FindByNameAsync(userDTO.RoleName);
                    if (role is not null)
                    {
                        var createUser = await _userManager.CreateAsync(newUser!, userDTO.Password);
                        await _userManager.AddToRoleAsync(newUser, userDTO.RoleName);
                        if (!createUser.Succeeded)
                        {
                            var errors = string.Join(", ", createUser.Errors.Select(e => e.Description));
                            return BadRequest(new { Message = $"Error occurred: {errors}" });
                        }
                        var branch = _dataContext.tblO_Branch.Where(b => b.Id == newUser.BranchId).First();
                        var manger = _dataContext.tblO_Staff.Where(x => x.Id == branch.BranchMangerId).ToList();
                        var data = new {
                                            user = newUser,
                                            role=role, 
                                            branch =new {branch=branch,manager= manger }
                                       };
                        return Ok(new { Message = "Account Created", Data = data });
                    }
                    else { return BadRequest(new { Message = "Please Select Role!" }); }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return BadRequest(new { Message = $"Error occurred: {ex.InnerException.Message}" });
                    }
                    // Log the exception message
                    return BadRequest(new { Message = $"Error occurred: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Replace a single user", Description = "")]
        public async Task<IActionResult> UpdateUser(string id, UserDTO userDTO)
        {
            try
            {
                if (userDTO is null) return BadRequest(new { Message = "Model is Empty !" });

                var user = await _userManager.FindByIdAsync(id);
                if (user is null) return BadRequest(new { Message = "Invalid User!" });

                var checkDupUsername = await _userManager.FindByNameAsync(userDTO.UserName);
                if (checkDupUsername is not null && checkDupUsername.Id != id)
                {
                    return BadRequest(new { Message = "Username already taken!" });
                }

                user.UserName = userDTO.UserName;
                user.Email = userDTO.Email;
                user.BranchId = userDTO.BranchId;
                user.AllowResetPassword = userDTO.AllowResetPassword;
                user.Active = userDTO.Active;

                if (userDTO.RoleName != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                        return BadRequest(new { Message = $"Error occurred: {errors}" });
                    }

                    var addResult = await _userManager.AddToRoleAsync(user, userDTO.RoleName);
                    if (!addResult.Succeeded)
                    {
                        return BadRequest(new { Message = "Failed to add new user role !" });
                    }
                }

                var update = await _userManager.UpdateAsync(user);
                if (!update.Succeeded)
                {
                    var errors = string.Join(", ", update.Errors.Select(e => e.Description));
                    return BadRequest(new { Message = $"Error occurred: {errors}" });
                }
               
                var branch = _dataContext.tblO_Branch.Where(b => b.Id == user.BranchId).First();
                var role = await _roleManager.FindByNameAsync(userDTO.RoleName);
                var manger = _dataContext.tblO_Staff.Where(x => x.Id == branch.BranchMangerId).ToList();
                var data = new
                {
                    user = user,
                    role = role,
                    branch = new { branch = branch, manager = manger }
                    
                };
                return Ok(new { Message = "Account Created", Data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data to the database.");
            }
        }
        [HttpPatch("{id}")]
        [SwaggerOperation(Summary = "update status of user ==delete level admin", Description = "Master Admin can update all day")]
        public async Task<IActionResult> UpdateUser(string id, string UserId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user is null) return BadRequest(new { Message = "Invalid User!" });
                if (user.Created_At.Date != DateTime.Now.Date)
                {
                    var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(UserId);
                    if (checkRole?.Name != "master admin")
                        return BadRequest(new { Message = "You Don't have permission to delete. Please contact to master admin" });
                }
                user.Active = false;
                user.Created_By = UserId;

                var update = await _userManager.UpdateAsync(user);
                if (!update.Succeeded)
                {
                    var errors = string.Join(", ", update.Errors.Select(e => e.Description));
                    return BadRequest(new { Message = $"Error occurred: {errors}" });
                }
                return Ok(new { Message = "User has been deleted successfully"});
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data to the database.");
            }
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Master Admin Only", Description = "Delete from DB")]
        public async Task<IActionResult> DeleteUser(string id,string UserId)
        {
            var checkrole=await _dataContext.ViewAuth_UserRole.FindAsync(UserId);
            if(checkrole.Name!="master admin")
                return BadRequest(new { Message = "You Don't have permission to delete. Please contact to Master Admin" });
            var findUser = await _userManager.FindByIdAsync(id);
            if (findUser is null) return NotFound(new { error = "User not found" });
            var deleteRole = await _userManager.DeleteAsync(findUser);
            if (deleteRole.Succeeded)
            {
                return Ok(new { result = $"User has been delete successfully" });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "User deletion failed" });
        }
    }
}