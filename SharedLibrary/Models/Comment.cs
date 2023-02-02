using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.Models
{
    public class Comment
    {
#pragma warning disable
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual Movie Movie { get; set; }

        [Required]
        public virtual User User { get; set; }
        
        public string? Content { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int Like { get; set; }

        public virtual Comment? ResponseTo { get; set; }
        public virtual List<Comment>? Responses { get; set; }
	}
}
