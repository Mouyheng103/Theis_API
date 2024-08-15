﻿using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Branch 

    {
        [Key]
        public int Id {  get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; } = string.Empty;

        public string? ProvinceCode { get; set; }
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
        public int? BranchMangerId { get; set; }
        public DateTime Created_at { get; set; }
        public string created_by { get; set; } = string.Empty;
        public bool IsActive {  get; set; }
    }
    public class Position
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string Created_By { get; set; } = string.Empty;
        public DateTime Created_At { get;set; }
        public string Updated_By { get; set; } = string.Empty;
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
