using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Models
{
#nullable disable
    public class Trailer
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }

        [Required]
        public string Clip { get; set; }

        public int Length { get; set; }
    }
}
