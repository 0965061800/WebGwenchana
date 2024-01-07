using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGwenchana.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }
        [ForeignKey("Order")]
        public int OrderID { get; set; }
        public virtual Order? Order { get; set; }
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public virtual Product? Product { get; set; }
        public int OrderNumber {  get; set; }
        public int Amount { get; set; }
        public int Discount {  get; set; }
        public int TotalMoney {  get; set; }
        public DateTime CreateDate { get; set; }
		public int? Price { get; set; }
	}
}
