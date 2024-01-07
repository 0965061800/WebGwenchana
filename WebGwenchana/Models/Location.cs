using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace WebGwenchana.Models
{
    public class Location
    {
        [Key]
        public int LocationID { get; set; }
        [StringLength(50)]
        public required string Name { get; set; }
		public int? Parent { get; set; }
		public int? Levels { get; set; }
		public string? Slug { get; set; }
		public string? NameWithType { get; set; }
		public string? Type { get; set; }

	}
}
