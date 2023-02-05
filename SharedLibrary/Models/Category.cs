using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Models
{
    public class Category
    {
#pragma warning disable
        public string Id { get; set; }
        public int IdNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
		public string? NormalizeName { get; set; }

		public string? Description { get; set; }

        public virtual List<Movie>? Movies { get; set; } 
    }
}
