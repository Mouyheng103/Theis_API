using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Branch 

    {
        [Key]
        public int Id {  get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; } = string.Empty;

        public int? ProvinceId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
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
    public class Position
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Kh_Name {  get; set; } = string.Empty;
        [Required]
        public string En_Name {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; }
        public Guid Created_By { get; set; }
        public DateTime Created_At { get;set; }
        public Guid Updated_By { get; set; }
        public DateTime Updated_At { get; set; }
    }
    //Location
    public class Province
    {
        [Key, Required]
        public string ProvinceCode { get; set; } = string.Empty;
        public string ProvinceName { get; set; } = string.Empty;
        public bool? Active {  get; set; }
    }
    public class District
    {
        [Key, Required]
        public string DistrictCode { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string ProvinceCode { get; set; } = string.Empty;
    }
    public class Commune
    {
        [Key, Required]
        public string CommuneCode { get; set; } = string.Empty;
        public string CommuneName { get; set; } = string.Empty;
        public string DistrictCode { get; set; } = string.Empty;
    }
    public class Village
    {
        [Key, Required]
        public string VillageCode {  get; set; } = string.Empty;
        public string VillageName { get; set; } = string.Empty;
        public string CommuneCode { get; set; } = string.Empty;
    }
    public class serviceResponses
    {
        public record class GeneralResponse(bool Flag, string Message);
        public record class LoginResponse(bool Flag, string Token,string UserId, string UserName, string RoleName, int BranchId, string Message);
    }
}
