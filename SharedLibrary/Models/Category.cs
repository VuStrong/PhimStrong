using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.Models
{
    public class Category
    {
#pragma warning disable
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
		public string? NormalizeName { get; set; }

		public string? Description { get; set; }

        public virtual List<Movie>? Movies { get; set; } 
    }
}
