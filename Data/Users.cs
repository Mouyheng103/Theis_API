using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Users : IdentityUser
    {
        public int BranchId { get; set; }
        public bool AllowResetPassword  { get; set; }
        public bool Active { get; set; }
        public DateTime Created_At { get; set; }
        public string Created_By { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } 
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
    public class Roles : IdentityRole
    {
        public string? Description { get; set; }
    }
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int BranchId { get; set; }
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Column("Name")]
        public string RoleName { get; set; } = string.Empty; 
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool AllowResetPassword { get; set; }
        public bool Active { get; set; }
        public DateTime Created_At { get; set; }
        public string Created_By { get; set; } = string.Empty;

    }
    public class ViewAuth_UserRole
    {
        [Key]
        public string UserId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
    public class ViewO_Users
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
    public class RoleDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
   
    public record UserSession(string? Id, string? UserName, string? Role);
    public class TokenRequest
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; }= string.Empty;
    }
}
