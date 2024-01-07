using System.ComponentModel.DataAnnotations;

namespace WebGwenchana.Models
{
    public class TransactStatus
    {
        [Key]
        public int TransactStatusID { get; set; }
        [StringLength(50)]
        public string? Status {  get; set; }
        public string? Description {  get; set; }
    }
}
