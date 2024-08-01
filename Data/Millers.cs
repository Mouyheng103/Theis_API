using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class Millers
    {
        [Key,Required]
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Name { get; set; } =string.Empty;
        public string? Description { get; set; }
        [Required,Phone,MaxLength(11)]
        public string Tel_1 { get; set; } = string.Empty;
        [Phone, MaxLength(11)]
        public string? Tel_2 { get; set; }
        [Phone,MaxLength(11)]
        public string? Tel_3 { get; set; }
        public string VillageCode { get; set; } = string.Empty;
        public bool Active { get; set; }
        public Guid Created_By { get; set; }
        public DateTime Created_At { get; set; }
        public Guid? Updated_By { get; set; }
        public DateTime? Updated_At { get; set; }
    }
}
