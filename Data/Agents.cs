using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Agents
    {

        [Key,Required]
        public int Id { get; set; }
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
        public int StaffId { get; set; }
        public int BranchId { get; set; }
        public string VillageCode { get; set; }
        public bool Active { get; set; }
        public Guid? Created_By { get; set; }
        public DateTime? Created_At { get; set; }
        public Guid? Updated_By { get; set; }
        public DateTime? Updated_At { get; set; }
    }
}
