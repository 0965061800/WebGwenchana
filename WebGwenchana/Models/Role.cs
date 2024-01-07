using System.ComponentModel.DataAnnotations;

namespace WebGwenchana.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        [StringLength(50)]
        public string? RoleName { get; set; }
        [StringLength(150)]
        public string? Description { get; set; }
        public string? Status {  get; set; }
    }
}
