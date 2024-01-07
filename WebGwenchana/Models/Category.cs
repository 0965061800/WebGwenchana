using System.ComponentModel.DataAnnotations;

namespace WebGwenchana.Models
{
    public class Category
    {
        [Key]
        public int CatID { get; set; }
        [StringLength(150)]
        public string? CatName { get; set; }
        [StringLength(1500)]
        public string? Description { get; set; }
        public int? ParentID { get; set; }
        public int? Levels { get; set; }
        [StringLength(1500)]
        public string? Photo { get; set; }
        public int? Order { get; set; }
        public string? Title { get; set; }
    }
}
