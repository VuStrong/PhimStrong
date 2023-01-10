using SharedLibrary.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Areas.Admin.Models
{
    public class MovieModel
    {
        public int Id { get; set; }

#pragma warning disable
        [Required(ErrorMessage = "Chưa nhập tên phim :()")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [DisplayName("Thời lượng phim")]
        public int? Length { get; set; }

        public int View { get; set; }

        [DisplayName("Ảnh phim")]
        public IFormFile? ImageFile { get; set; }

        public string? Trailer { get; set; }

        public string[]? Categories { get; set; }

        public string? Country { get; set; }

        [DisplayName("Đạo diễn")]
        public string[]? Directors { get; set; }
        [DisplayName("Diễn viên")]
        public string[]? Casts { get; set; }

        public float Rating { get; set; }

        [Required(ErrorMessage = "Chưa chọn loại phim")]
        [DisplayName("Loại phim")]
        public string Type { get; set; }

        public string[]? Videos { get; set; }
        [DisplayName("Số tập")]
        public int EpisodeCount { get; set; }
    }
}
