using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Users : IdentityUser
    {
        public int BranchId { get; set; }
    }
    public class Roles : IdentityRole
    {
        public string? Description { get; set; }
    }
    public class UserDTO
    {
        public string? Id { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public int BranchId { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Column("Name")]
        public string RoleName { get; set; } = string.Empty; //role name
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
    public class RoleDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
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
}
