using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Customers
    {
        [Key,MaxLength(10)]
        public string Id { get; set; } =string.Empty;
        [Required,MaxLength(10)]
        public string AgentID { get; set; } = string.Empty;
        public string Wife_Name { get; set; } = string.Empty;
        public string? Husband_Name { get; set; }
        [Phone,MaxLength(11)]
        public string Tel { get; set; } = string.Empty;
        [Required, MaxLength(8)]
        public string VillageCode { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public bool? BlackList { get; set; }
        public bool Active { get; set; }
        public string Created_By { get; set; } = string.Empty;
        public DateTime Created_At { get; set; }
        public string Updated_By { get; set; } = string.Empty;
        public DateTime Updated_At { get; set; }
    }
    public class ViewO_Customer
    {
        [Key, MaxLength(10)]
        public string Id { get; set; } = string.Empty;
        [MaxLength(10)]
        public string AgentID { get; set; } = string.Empty;
        public string? FirstNameM { get; set; }
        public string? LastNameM { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [Phone, MaxLength(11)]
        public string Tel { get; set; } = string.Empty;
        [MaxLength(8)]
        public string VillageCode { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public bool? BlackList { get; set; }
        public bool Active { get; set; }
        public string? Created_By { get; set; }
        public DateTime? Created_At { get; set; }
        public string? Updated_By { get; set; }
        public DateTime? Updated_At { get; set; }
        public string Address { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string Branch_Name { get; set; } = string.Empty;
    }
}
