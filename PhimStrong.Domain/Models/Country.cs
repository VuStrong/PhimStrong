using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Domain.Models
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

        public List<Movie>? Movies { get; set; } 
    }
}
