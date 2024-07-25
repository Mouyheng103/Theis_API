using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Millers
    {
        [Key,Required]
        public int AutoId { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Tel_1 { get; set; }
        public string? Tel_2 { get; set; }
        public string? Tel_3 { get; set; }
        public string VillageCode { get; set; }
        public bool Active { get; set; }
        public Guid Created_By { get; set; }
        public DateTime Created_At { get; set; }
        public Guid? Updated_By { get; set; }
        public DateTime? Updated_At { get; set; }
    }

}
