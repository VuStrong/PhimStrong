using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Models
{
    public class Country
    {
#pragma warning disable
        public string Id { get; set; }
        public int IdNumber { get; set; }

        [Required]
        public string Name { get; set; }
		public string? NormalizeName { get; set; }

		public string? About { get; set; }

        public virtual List<Movie>? Movies { get; set; } 
    }
}
