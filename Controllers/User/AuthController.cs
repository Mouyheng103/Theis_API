using API.Data;
using API.Function;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static API.Data.serviceResponses;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers.User
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly UserManager<Data.Users> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        private readonly IConfiguration _config;
        private readonly DataContext _dataContext;
        private readonly TokenService _tokenService;
        private readonly SignInManager<Data.Users> _signInManager;
        public AuthController(UserManager<Data.Users> userManager, RoleManager<Roles> roleManager, IConfiguration config, DataContext dataContext, TokenService tokenService, SignInManager<Data.Users> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _dataContext = dataContext;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _signInManager = signInManager;
        }
        
        [HttpPut("resetpassword"), Authorize]
        public async Task<IActionResult> ResetPasswordAsync(string userName, string branchName, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new Exception("User not found.");

            if (user.AllowResetPassword == false)
                return BadRequest(new { Message = "Please Contact to ADMIN !!!" });

            if (user.UserName != userName)
                return BadRequest(new { Message = "Username or Branch Not Correct !!!" });

            var branch = _dataContext.tblO_Branch.Where(b => b.Name.Contains(branchName)).FirstOrDefault();
            if (branch == null)
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
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(loginRequest.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return Unauthorized("Invalid username or password");

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginRequest.Password, false, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid login attempt");

            var tokenResponse = await _tokenService.GenerateToken(user);
            var getUserRole = await _userManager.GetRolesAsync(user);
            var branch = await _dataContext.tblO_Branch.Where(x => x.Id == user.BranchId).FirstAsync();
            var data = new
            {
                Token = tokenResponse.Token,
                Expiration = tokenResponse.Expiration,
                RefreshToken = tokenResponse.RefreshToken,
                User = new
                {
                    Id = user.Id,
                    UserName = loginRequest.UserName,
                    BranchId = branch,
                    Roles = getUserRole
                }
            };
            return Ok(new { Message = "Login successfully.", Data=data });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated.");
            }

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User identity name is not found.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Invalidate the refresh token
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            // Optionally, add the current access token to a blacklist
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _tokenService.AddTokenToBlacklist(token);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(500, "Logout failed. Could not update the user.");
            }

            // Sign out the user from the identity system
            await _signInManager.SignOutAsync();

            return Ok(new { message = "Logout successful" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRequest tokenRequest)
        {
            if (tokenRequest == null || string.IsNullOrEmpty(tokenRequest.Token) || string.IsNullOrEmpty(tokenRequest.RefreshToken))
                return BadRequest("Invalid client request");

            var principal = GetPrincipalFromExpiredToken(tokenRequest.Token);
            if (principal == null)
            {
                return BadRequest("Invalid token");
            }

            var username = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid refresh token");
            }
            var newToken = await _tokenService.GenerateToken(user);

            return Ok(new { Message = "New Token.", Data = newToken });

        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
    
}
