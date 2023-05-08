using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Domain.Models
{
    public class Cast
    {
#pragma warning disable
        public string Id { get; set; }
        public int IdNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
		public string? NormalizeName { get; set; }

        public string? Avatar { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? About { get; set; }

        public List<Movie>? Movies { get; set; }

    }
}
