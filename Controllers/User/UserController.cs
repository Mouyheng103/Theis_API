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
        [HttpPost("Register")]
        public async Task<GeneralResponse> CreateAccount(UserDTO userDTO)
        {
            try
            {
                if (userDTO is null) return new GeneralResponse(false, "Model is empty");
                var newUser = new Data.Users()
                {
                    UserName = userDTO.UserName,
                    Email = userDTO.Email,
                    PasswordHash = userDTO.Password,
                    BranchId = userDTO.BranchId

                };
                var user = await _userManager.FindByNameAsync(newUser.UserName);
                if (user is not null) return new GeneralResponse(false, "User registered already");
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
                            return new GeneralResponse(false, $"Error occurred: {errors}");
                        }
                        return new GeneralResponse(true, "Account Created");
                    }
                    else { return new GeneralResponse(false, "Please Select Role!"); }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return new GeneralResponse(false, $"Error occurred: {ex.InnerException.Message}");
                    }
                    // Log the exception message
                    return new GeneralResponse(false, $"Error occurred: {ex.Message}");

                }
            }
            catch (SqlException ex)
            {
                return new GeneralResponse(false, $"Error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, $"Error occurred: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> LoginAccount(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                return NotFound(new { Message = "Model is Empty" });

            var getUser = await _userManager.FindByNameAsync(loginDTO.UserName);
            if (getUser is null)
                return NotFound(new { Message = "User not found" });

            bool checkUserPasswords = await _userManager.CheckPasswordAsync(getUser, loginDTO.Password);
            if (!checkUserPasswords)
                return BadRequest(new { Message = "Invalid username/password" });

            var getUserRole = await _userManager.GetRolesAsync(getUser);
            var getBranchId = await _dataContext.AspNetUsers.Where(e => e.UserName == loginDTO.UserName).Select(e => e.BranchId).FirstOrDefaultAsync();
            var userSession = new UserSession(getUser.Id, getUser.UserName, getUserRole.First());
            string token = GenerateToken(userSession);
            return new LoginResponse(true, token, loginDTO.UserName, getUserRole.First(), getBranchId, "Login Success!");
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