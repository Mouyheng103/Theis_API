using System.ComponentModel.DataAnnotations;

namespace API.Data
{
    public class ShareToBranch
    {
        [Key]
        public int Id { get; set; }
        public string YearId { get; set; }=string.Empty;
        public int MillerId { get; set; }
        public int BranchId { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
    }
    public class TakeRice
    {
        [Key]
        public int Id { get; set; }
        public string YearId { get; set; } = string.Empty;
        public int MillerId { get; set; }
        public int Section { get; set; }
        public decimal Stock { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
    }

}
