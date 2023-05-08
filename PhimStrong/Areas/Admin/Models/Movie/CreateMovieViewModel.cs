using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PhimStrong.Areas.Admin.Models.Movie
{
	public class CreateMovieViewModel
	{
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

#nullable disable
		public CreateMovieViewModel() { }

		public CreateMovieViewModel(string name, string translateName, string type, string status)
		{
			Name = name;
			TranslateName = translateName;
			Type = type;
			Status = status;
		}
	}
}
