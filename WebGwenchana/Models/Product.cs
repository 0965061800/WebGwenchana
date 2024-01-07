using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace WebGwenchana.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        [StringLength(150)]
        public string? ProductName { get; set; }
        [StringLength(1000)]
        public string? ProductShortDesciption { get; set; }

        [StringLength(30000)]
        public string? ProductDescription { get; set; }
        [ForeignKey("Category")]
        public int? CatID { get; set; }
        public virtual Category? Category { get; set; }
        [Column(TypeName = "decimal(20,2)")]
        public decimal? ProductPrice { get; set; }
        [Column(TypeName = "decimal(2,2)")]
        public decimal? ProductDiscount { get; set; }
        [StringLength(255)]
        public string? ProductPhoto { get; set; }
        public string? ProductCollection {  get; set; }
        [StringLength(255)]
        public string? ProductTitle { get; set; }
        public DateTime? ProductDateCreated { get; set; }
        public bool BestSellers { get; set; }
        public bool Active { get; set; }
        public string? Tags { get; set; }
        public string? MetaDesc { get; set; }
        public string? MetaKey { get; set; }
        [Required]
        public int UnitsInStock {  get; set; }
        [Required]
        public int? SizeId { get; set; }
        
    }
}
