using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Agents
    {

        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        [Phone]
        public string? Tel_1 { get; set; }
        [Phone]
        public string? Tel_2 { get; set; }
        [Phone]
        public string? Tel_3 { get; set; }
        public int? PositionId { get; set; }
        public int Commission { get; set; }
        public int BranchId { get; set; }
        public string VillageCode { get; set; }
        public bool Active { get; set; }
        public Guid? Created_By { get; set; }
        public DateTime? Created_At { get; set; }
        public Guid? Updated_By { get; set; }
        public DateTime? Updated_At { get; set; }
    }
    public class ViewO_Agents
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string Tel_1 { get; set; }
        public string Tel_2 { get; set; }
        public string Tel_3 { get; set; }
        public int PositionId { get; set; }
        public string Position_Name { get; set; }
        public int Commission { get; set; }
        public int BranchId { get; set; }
        public string Branch_Name { get; set; }
        public string VillageCode { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public Guid Created_By { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public Guid Updated_By { get; set; }
    }

}
