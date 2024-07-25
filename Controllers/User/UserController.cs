using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using static API.Data.serviceResponses;
using Microsoft.AspNetCore.Identity.UI.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers.User
{
    [Route("api/user")]
    [ApiController]

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
        [HttpGet("getuser")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var result = from user in _dataContext.Users
                             join userRole in _dataContext.UserRoles on user.Id equals userRole.UserId
                             join role in _dataContext.Roles on userRole.RoleId equals role.Id
                             join branch in _dataContext.tblO_Branch on user.BranchId equals branch.Id
                             select new
                             {
                                 UserId = user.Id,
                                 UserName = user.UserName,
                                 Email = user.Email,
                                 BranchId = branch.Id,
                                 BranchName = branch.Name,
                                 Role = role.Name,
                                 AllowReset=user.AllowResetPassword,
                                 Active=user.Active,
                                 Created_At=user.Created_At,
                                 Created_by=user.Created_By,
                             };

                var userList = result.ToList();
                if (userList is null) { return NotFound("No Branch !"); }
                return Ok(userList);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving data from the database.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
        [HttpPost("register")]
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
                        AllowResetPassword=false,
                        Active=true,
                        Created_At = DateTime.UtcNow,
                        Created_By=userDTO.Created_By
                    
                    };
                var user = await _userManager.FindByNameAsync(newUser.UserName);
                if (user is not null) return BadRequest(new { Message = "User registered already" });
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
                        var branch= _dataContext.tblO_Branch.Where(b =>b.Id==newUser.BranchId).ToList();
                        var createdUserData = new
                        {
                            newUser.Id,
                            newUser.UserName,
                            newUser.Email,
                            branch, 
                            newUser.Active,
                            newUser.Created_At,
                            newUser.Created_By,
                            Role = userDTO.RoleName
                        };
                        return Ok(new { Message = "Account Created", Data = createdUserData });
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
            catch (SqlException ex)
            {
                return BadRequest(new { Message = $"Error occurred: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error occurred: {ex.Message}" });
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UserDTO userDTO)
        {
            try
            {
                if (userDTO is null) return BadRequest(new { Message = "Model is Empty !" });

                var user = await _userManager.FindByIdAsync(userDTO.Id);
                if (user is null) return BadRequest(new { Message = "Invalid User!" });

                var checkDupUsername = await _userManager.FindByNameAsync(userDTO.UserName);
                if (checkDupUsername is not null && checkDupUsername.Id != userDTO.Id)
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
                var branch = _dataContext.tblO_Branch.Where(b => b.Id == user.BranchId).ToList();

                var updatedUserData = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    branch,
                    user.AllowResetPassword,
                    user.Active,
                    Role = userDTO.RoleName
                };

                return Ok(new { Message = "Account Updated", Data = updatedUserData });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { Message = $"Error occurred: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error occurred: {ex.Message}" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginAccount(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                return NotFound(new { Message = "Model is Empty" });

            var getUser = await _userManager.FindByNameAsync(loginDTO.UserName);
            if (getUser is null)
                return NotFound(new { Message = "User not found" });
            if(getUser.Active==false)
                return  BadRequest(new { Message = "Account is closed! " });
            bool checkUserPasswords = await _userManager.CheckPasswordAsync(getUser, loginDTO.Password);
            if (!checkUserPasswords)
                return BadRequest(new { Message = "Invalid username/password" });

            var getUserRole = await _userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.UserName, getUserRole.First()); 
            string token = GenerateToken(userSession);
            return new LoginResponse(true, token,getUser.Id, loginDTO.UserName, getUserRole.First(), getUser.BranchId, "Login Success!");
        }
        [HttpPut("resetpassword")]
        public async Task<IActionResult> ResetPasswordAsync(string userName,string branchName, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new Exception("User not found.");

            if (user.AllowResetPassword == false)
                return BadRequest(new { Message = "Please Contact to ADMIN !!!" });

            if(user.UserName != userName)
                return BadRequest(new { Message = "Username or Branch Not Correct !!!" });

            var branch = _dataContext.tblO_Branch.Where(b => b.Name.Contains(branchName)).FirstOrDefault();
            if(branch == null)
                return BadRequest(new { Message = "Username or Branch Not Correct !!!" });

            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
                return BadRequest(new { Message = "Error removing current password.", Errors = removePasswordResult.Errors });

            // Add the new password
            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (addPasswordResult.Succeeded)
                return Ok(new { Message = "Password reset successfully." });
            return BadRequest(new { Message = "Error resetting password.", Errors = addPasswordResult.Errors });
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var findrole = await _userManager.FindByIdAsync(id);
            if (findrole is null) return NotFound(new { error = "User not found" });
            var deleteRole = await _userManager.DeleteAsync(findrole);
            if (deleteRole.Succeeded)
            {
                return Ok(new { result = $"User has been delete successfully" });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "User deletion failed" });
        }
        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}