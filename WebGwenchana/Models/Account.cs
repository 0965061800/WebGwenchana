using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGwenchana.Models
{
    public class Account
    {
            [Key]
            public int AccountID { get; set; }
            [StringLength(20)]
            public string? Phone { get; set; }
            [StringLength(50)]
            public string? Email { get; set; }
            [Required]
            [StringLength(100)]
            public required string Password { get; set; }

            [StringLength(100)]
            public string? FullName { get; set; }
            [StringLength(100)]
            [ForeignKey("Role")]            
            public int RoleId { get; set; }
            public virtual Role? Role { get; set; }
            public DateTime CreateDate { get; set; }
            public string? Status { get; set; }
        }
}
