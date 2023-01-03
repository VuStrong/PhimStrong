using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.Models
{
    public class Comment
    {
#pragma warning disable
        [Key]
        public int Id { get; set; }

        [Required]
        public Movie Movie { get; set; }

        [Required]
        public User User { get; set; }
        
        public string? Content { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int Like { get; set; }

        public Comment? ResponseTo { get; set; }
        public List<Comment>? Responses { get; set; }
    }
}
