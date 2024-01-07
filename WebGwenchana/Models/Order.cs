using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace WebGwenchana.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public virtual Customer? Customer { get; set; }
        public DateTime OrderDate {  get; set; }
        [ForeignKey("TransactionStatus")]
        public int TransactStatusID { get; set; }
        public virtual TransactStatus? TransactStatus { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public int TotalMoney {  get; set; }
        public string? Note { get; set; }
        public string? Address { get; set; }
		public int? LocationId { get; set; }
		public int? District { get; set; }
		public int? Ward { get; set; }
	}
}
