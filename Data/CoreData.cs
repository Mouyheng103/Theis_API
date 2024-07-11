using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    
    public class Branch 

    {
        [Key]
        public int Id {  get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }
      
        public int? ProvinceId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Phone]
        public string? PhoneNumber {  get; set; }
        public string? InternetNumber { get; set; }
        public string? Location { get; set; }
        public Guid? BranchMangerId { get; set; }
        public DateTime Created_at { get; set; }
        public Guid created_by { get; set; }
        public bool IsActive {  get; set; }
    }
    public class serviceResponses
    {
        public record class GeneralResponse(bool Flag, string Message);
        public record class LoginResponse(bool Flag, string Token, string UserName, string RoleName, int BranchId, string Message);
    }
}
