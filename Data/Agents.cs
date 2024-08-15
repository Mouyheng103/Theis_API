using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Agents
    {

        [Key,MaxLength(10)]
        public string Id { get; set; } = string.Empty;
        [Required,MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1)]
        public string Gender { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        [Phone]
        public string? Tel_1 { get; set; }
        [Phone]
        public string? Tel_2 { get; set; }
        [Phone]
        public string? Tel_3 { get; set; }
        [Required]
        public int PositionId { get; set; }
        public int Commission { get; set; }
        [Required]
        public int BranchId { get; set; }
        public string VillageCode { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string? Created_By { get; set; }
        public DateTime Created_At { get; set; }
        public string? Updated_By { get; set; }
        public DateTime Updated_At { get; set; }
    }
    public class ViewO_Agents
    {
        [Key, MaxLength(10)]
        public string Id { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1)]
        public string Gender { get; set; } = string.Empty;
        public DateOnly? DOB { get; set; }
        [Phone]
        public string? Tel_1 { get; set; }
        [Phone]
        public string? Tel_2 { get; set; }
        [Phone]
        public string? Tel_3 { get; set; }
        [Required]
        public int PositionId { get; set; }
        public int Commission { get; set; }
        [Required]
        public int BranchId { get; set; }
        public string VillageCode { get; set; } = string.Empty;
        public bool Active { get; set; }
        public string? Created_By { get; set; }
        public DateTime? Created_At { get; set; }
        public string? Updated_By { get; set; }
        public DateTime? Updated_At { get; set; }
        public string Position_Name { get; set; } = string.Empty;
        public string Branch_Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

}
