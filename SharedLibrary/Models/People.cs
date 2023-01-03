using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Models
{
    public class People
    {
#pragma warning disable
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [NotMapped]
        public int Age 
        { 
            get
            {
                return int.Parse(Math.Floor((double)(DateTime.Now - this.DateOfBirth).Value.Days / 365).ToString());
            } 
        }

        [NotMapped]
        public List<Movie> DirectedMovies { get; set; }
        [NotMapped]
        public List<Movie> JoinedMovies { get; set; }
    }
}
