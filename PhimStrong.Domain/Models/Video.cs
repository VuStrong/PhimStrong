using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Domain.Models
{
#pragma warning disable
    public class Video
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public Movie Movie { get; set; }

        public string? VideoUrl { get; set; }

        public int? Episode { get; set; }
        [DisplayName("Thời lượng video")]
        public int? Length { get; set; }
    }
}
