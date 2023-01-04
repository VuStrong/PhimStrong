using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Models
{
    public class Movie
    {
#pragma warning disable
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int? Length { get; set; }

        public int View { get; set; }

        [NotMapped]
        [DisplayName("Ảnh phim")]
        public IFormFile? ImageFile { get; set; }
        public string? Image { get; set; }

        public string? Trailer { get; set; }

        public Category? Category { get; set; }

        public Country? Country { get; set; }

        [NotMapped]
        public List<People> Directors { get; set; }
        [NotMapped]
        public List<People> Casts { get; set; }

        public float Rating { get; set; }

        [Required]
        public Type Type { get; set; }
    }

    public enum Type
    {
        PhimLe,PhimBo
    }
}