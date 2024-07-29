using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Customers
    {
        [Key]
        public string Id { get; set; } =string.Empty;
        public string AgentID { get; set; }
        public string? FirstNameM { get; set; }
        public string? LastNameM { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Phone]
        public string Tel { get; set; }
        public string VillageCode { get; set; }
        public DateTime? DOB { get; set; }
        public bool? BlackList { get; set; }
        public bool Active { get; set; }
        public Guid? Created_By { get; set; }
        public DateTime? Created_At { get; set; }
        public Guid? Updated_By { get; set; }
        public DateTime? Updated_At { get; set; }
    }
    public class ViewO_Customer
    {
        [Key]
        public string Id { get; set; }
        public string AgentID { get; set; }
        public string FirstNameM { get; set; }
        public string LastNameM { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Phone]
        public string Tel { get; set; }
        public string VillageCode { get; set; }
        public DateOnly DOB { get; set; }
        public bool BlackList { get; set; }
        public bool Active { get; set; }
        public Guid Created_By { get; set; }
        public DateTime Created_At { get; set; }
        public Guid Updated_By { get; set; }
        public DateTime Updated_At { get; set; }
        public string Address { get; set; }
        public string AgentName { get; set; }
        public int BranchId { get; set; }
        public string Branch_Name { get; set; }
    }
}
