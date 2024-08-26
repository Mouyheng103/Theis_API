using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Created_By { get; set; } = string.Empty;
        public DateTime Created_At { get; set; }
        public string Updated_By { get; set; } = string.Empty;
        public DateTime Updated_At { get; set; }
    }
    public class RicePurchase
    {
        [Key]
        public int PurchaseID { get; set; } 
        public int YearID { get; set; }
        public int MillerID { get; set; }
        public int Section { get; set; }
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Weight { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCost { get; set; }
        public int PaymentStatus { get; set; }
        public string Created_By { get; set; } = string.Empty;
        public DateTime Created_At { get; set; }
        public string Updated_By { get; set; } =string.Empty;
        public DateTime? Updated_At { get; set; }
    }
    public class RicePurchasePayment
    {
        [Key]
        public int PaymentID { get; set; }
        public int PurchaseID { get; set; }
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountPaid { get; set; }
        public int PaymentMethod { get; set; } 
        public string Created_By { get; set; } = string.Empty;
        public DateTime Created_At { get; set; }
        public string Updated_By { get; set; } = string.Empty;
        public DateTime? Updated_At { get; set; }
    }
    public class PaymentComponent
    {
        [Key]
        public int Id { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
