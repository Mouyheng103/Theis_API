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
    public class CollectMoney
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string ProvideId { get; set; } =string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public int StaffId { get; set; }
        public decimal Recieve { get; set; }
        public decimal Balance { get; set; }
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
    }
    public class CusLoan
    {
        [Key]
        public int Id { get; set; }
        public string ProvideId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public int StaffId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
    }

}
