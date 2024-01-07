using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGwenchana.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        [StringLength(255)]
        public string? FullName { get; set; }
        public DateTime? Birthday { get; set; }
        [StringLength(3000)]
        public string? Avatar { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }
        [StringLength(150)]
        public string? Email { get; set; }
        [StringLength(12)]
        public string? Phone {  get; set; }
        [StringLength(50)]
        public string? Password { get; set; }
        [ForeignKey("Location")]
        public int? LocationID {  get; set; }
        public virtual Location? Location { get; set; } 
        public bool Active { get; set; }
		public DateTime? CreateDate { get; set; }
		public string? Salt { get; set; }
		public int? Ward { get; set; }
		public DateTime? LastLogin { get; set; }
		public int? District { get; set; }
	}
}
