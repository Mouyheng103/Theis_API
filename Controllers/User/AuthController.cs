using API.Data;
using API.Function;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static API.Data.serviceResponses;

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
        //[HttpPost("login")]
        //public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginDTO loginDTO)
        //{

        //    if (loginDTO == null)
        //        return NotFound(new { Message = "Model is Empty" });

        //    var getUser = await _userManager.FindByNameAsync(loginDTO.UserName);
        //    if (getUser is null)
        //        return NotFound(new { Message = "User not found" });
        //    if (getUser.Active == false)
        //        return BadRequest(new { Message = "Account is closed! " });
        //    bool checkUserPasswords = await _userManager.CheckPasswordAsync(getUser, loginDTO.Password);
        //    if (!checkUserPasswords)
        //        return BadRequest(new { Message = "Invalid username/password" });

        //    var getUserRole = await _userManager.GetRolesAsync(getUser);
        //    var userSession = new UserSession(getUser.Id, getUser.UserName, getUserRole.First());
        //    string token = GenerateJwtToken(userSession);
        //    return new LoginResponse(true, token, getUser.Id, loginDTO.UserName, getUserRole.First(), getUser.BranchId, "Login Success!");
        //}
        //private string GenerateJwtToken(UserSession user)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //        }),
        //        Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Jwt:DurationInMinutes"])),
        //        Audience = _config["Jwt:Audience"],  // Add this line
        //        Issuer = _config["Jwt:Issuer"],
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}
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
            var data = new { tokenResponse,user.Id,loginRequest.UserName,user.BranchId };
            return Ok(data);
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

            return Ok(newToken);
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
