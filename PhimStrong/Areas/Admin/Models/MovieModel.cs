using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Areas.Admin.Models
{
    public class MovieModel
    {
#nullable disable
        public string Id { get; set; }

#pragma warning disable
        [Required(ErrorMessage = "Chưa nhập tên phim :()")]
        public string Name { get; set; }

		[Required(ErrorMessage = "Chưa nhập :()")]
		public string TranslateName { get; set; }

		public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [DisplayName("Thời lượng phim")]
        public int? Length { get; set; }

        public int View { get; set; }

        [DisplayName("Ảnh phim")]
        public IFormFile? ImageFile { get; set; }
        public string? Image { get; set; }

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

        public string? Tags { get; set; }

		[Required(ErrorMessage = "Chưa chọn trạng thái")]
		[DisplayName("Trạng thái")]
		public string Status { get; set; }

        public string? CategoryToString
        {
            get
            {
                if (this.Categories != null)
                {
                    return String.Join(",", this.Categories);
                }
                else
                {
                    return null;
                }
            }
        }

        public string? CastToString
        {
            get
            {
                if (this.Casts != null)
                {
                    return String.Join(",", this.Casts);
                }
                else
                {
                    return null;
                }
            }
        }

        public string? DirectorToString
        {
            get
            {
                if (this.Directors != null)
                {
                    return String.Join(",", this.Directors);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
