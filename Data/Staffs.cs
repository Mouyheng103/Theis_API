using System.ComponentModel.DataAnnotations;

public class Staffs
{
    [Key,Required]
    public int Id { get; set; }
    [Required]
    public int BranchId { get; set; }
    [Required]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;
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
    public string? UserId { get; set; }
    public string? VillageCode { get; set; }
    public DateOnly HireDate { get; set; }
    public decimal BaseSalary { get; set; }

    [Required]
    public bool Active { get; set; }
    public bool BlackList { get; set; }
    public string Created_By { get; set; } = string.Empty;
    public DateTime Created_At { get; set; }
    public string Updated_By { get; set; } = string.Empty;
    public DateTime Updated_At { get; set; }


}
public class SalaryChange
{
    [Key]
    public int Id { get; set; }
    public int StaffId { get; set; }
    public decimal Previous_Salary { get; set; }
    public decimal Increase_Salary { get; set; }
    public decimal New_Salary { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int Section {  get; set; }
    public string Created_By { get; set; } = string.Empty;
    public DateTime Created_At { get; set; }
    public string Updated_By { get; set; } = string.Empty;
    public DateTime Updated_At { get; set; }
}
public class Salary
{
    [Key]
    public int Id { get; set; }
    public int StaffId { get; set; }
    public decimal Gasoline { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal Bonuses { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Created_By { get; set; } = string.Empty;
    public DateTime Created_At { get; set; }
    public string Updated_By { get; set; } = string.Empty;
    public DateTime Updated_At { get; set; }
}
public class Payroll
{
    [Key]
    public int PayrollId { get; set; }
    public int StaffId { get; set; }
    public DateTime PayrollDate { get; set; }
    public string PayPeriod { get; set; } = string.Empty;
    public decimal NetSalary { get; set; }
    public string PayrollStatus { get; set; } = string.Empty;
    public string Created_By { get; set; } = string.Empty;
    public DateTime Created_At { get; set; }
    public string Updated_By { get; set; } = string.Empty;
    public DateTime Updated_At { get; set; }
}