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

        [Required(ErrorMessage = "Chưa nhập tên phim :()")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [DisplayName("Thời lượng phim")]
        public int? Length { get; set; }

        public int View { get; set; }

        public string? Image { get; set; }


        [ForeignKey("TrailerId")]
        public Trailer? Trailer { get; set; }

        public List<Category>? Categories { get; set; }

        public Country? Country { get; set; }

        [DisplayName("Đạo diễn")]
        public List<Director>? Directors { get; set; }
        [DisplayName("Diễn viên")]
        public List<Cast>? Casts { get; set; }

        public float Rating { get; set; }

        [Required(ErrorMessage = "Chưa chọn loại phim")]
        [DisplayName("Loại phim")]
        public string Type { get; set; }

        public List<Video>? Videos { get; set; }
        [DisplayName("Số tập")]
        public int EpisodeCount { get; set; }
    }
}