using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGwenchana.Models
{
    public class SizePrice
    {
        [Key]
        public int SizePriceID { get; set; }
        [ForeignKey("Size")]

        public int SizeId { get; set; }
        public virtual Size? Size { get; set; }
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public virtual Product? Product { get; set; }
        [Column(TypeName = "decimal(20,2)")]
        public decimal? ProductPrice { get; set; }
        public bool Active { get; set; }
    }
}
