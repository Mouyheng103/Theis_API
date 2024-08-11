
using System.ComponentModel.DataAnnotations;

public class Staffs
{
    [Key,Required]
    public int StaffId { get; set; }
    [Required]
    public int BranchId { get; set; }
    [Required]
    [StringLength(30)]
    public string Kh_Name { get; set; } = string.Empty;
    [Required]
    [StringLength(30)]
    public string En_Name { get; set; } = string.Empty;
    [Required]
    [StringLength(1)]
    public string Gender { get; set; } = string.Empty;
    [Phone]
    [StringLength(11)]
    public string? Tel_Cellcard { get; set; }
    [Phone]
    [StringLength(11)]
    public string? Tel_Smart { get; set; }
    [Phone]
    [StringLength(11)]
    public string? Tel_Metfone { get; set; }
    [Required]
    public int PositionId { get; set; }
    public Guid? UserId { get; set; }
    public string? VillageCode { get; set; }
    [Required]
    public bool Active { get; set; }
    public bool? BlackList { get; set; }
    public Guid Created_By { get; set; }
    public DateTime Created_At { get; set; }
    public Guid Updated_By { get; set; }
    public DateTime Updated_At { get; set; }
}
