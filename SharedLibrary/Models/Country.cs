using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.Models
{
    public class Country
    {
#pragma warning disable
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
		public string? NormalizeName { get; set; }

		public string? About { get; set; }

        public virtual List<Movie>? Movies { get; set; } 
    }
}
