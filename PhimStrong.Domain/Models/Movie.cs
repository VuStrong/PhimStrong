using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhimStrong.Domain.Models
{
    public class Movie
    {
#pragma warning disable
        public string Id { get; set; }
        
        public int IdNumber { get; set; }

        [Required(ErrorMessage = "Chưa nhập tên phim :()")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Chưa nhập :()")]
        public string TranslateName { get; set; }

        public string? NormalizeName { get; set; }
        public string? NormalizeTranslateName { get; set; }

		public string? Description { get; set; }

		public DateTime? ReleaseDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        public bool ShowInSlide { get; set; }

        [DisplayName("Thời lượng phim")]
        public int? Length { get; set; }

        public int View { get; set; }

        public string? Image { get; set; }

        public string? Trailer { get; set; }

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

        public List<User>? LikedUsers { get; set; }
        public List<Comment>? Comments { get; set; }

		public List<Tag>? Tags { get; set; }

		[Required(ErrorMessage = "Chưa chọn trạng thái")]
		[DisplayName("Trạng thái")]
		public string Status { get; set; }

		[NotMapped]
		public string StatusToString
		{
			get
			{
				if (this.Type == "Phim lẻ") return this.Status;
				else return "Tập " + this.EpisodeCount;
			}
		}
	}
}