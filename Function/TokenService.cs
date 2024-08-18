using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Function
{
    public class TokenService
    {
        private readonly UserManager<Users> _userManager;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;


        public TokenService(UserManager<Users> userManager, IConfiguration configuration, IMemoryCache cache)
        {
            _userManager = userManager;
            _configuration = configuration;
            _cache = cache;
        }
        public Task AddTokenToBlacklist(string token)
        {
            var expiry = DateTime.Now.AddHours(1); 
            _cache.Set(token, true, expiry);
            return Task.CompletedTask;
        }
        public Task<bool> IsTokenBlacklisted(string token)
        {
            return Task.FromResult(_cache.TryGetValue(token, out _));
        }
        public async Task<TokenResponse> GenerateToken(Users user)
        {
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
             new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:DurationInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return new TokenResponse
            {
                Token = jwtToken,
                Expiration = token.ValidTo,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }

}
