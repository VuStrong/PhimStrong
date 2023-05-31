using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PhimStrong.Domain.Models;

namespace PhimStrong.Areas.Admin.Models.Movie
{
	public class EditMovieViewModel
	{
		public string Id { get; set; }

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
        public string? CategoryToString { get; set; }

        public string? Country { get; set; }

		[DisplayName("Đạo diễn")]
		public string[]? Directors { get; set; }
        public string? DirectorToString { get; set; }

        [DisplayName("Diễn viên")]
		public string[]? Casts { get; set; }
        public string? CastToString { get; set; }

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

#pragma warning disable
        public EditMovieViewModel() { }

		public EditMovieViewModel(string id, string image, string name, string translateName, string type, string status)
		{
			Id = id;
			Image = image;
			Name = name;
			TranslateName = translateName;
			Type = type;
			Status = status;
		}

		public static EditMovieViewModel FromMovie(Domain.Models.Movie movie)
		{
            EditMovieViewModel model = new()
            {
                Id = movie.Id,
                Name = movie.Name,
                TranslateName = movie.TranslateName,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Length = movie.Length,
                View = movie.View,
                Type = movie.Type,
                EpisodeCount = movie.EpisodeCount,
                Image = movie.Image,
                Rating = movie.Rating,
                Status = movie.Status,
                Trailer = movie.Trailer,
            };

            if (movie.Tags != null)
            {
                model.Tags = String.Join(",", movie.Tags.Select(t => t.TagName));
            }

            if (movie.Categories != null)
            {
                model.Categories = movie.Categories.Select(c => c.Id).ToArray();

                model.CategoryToString = String.Join(", ", movie.Categories.Select(c => c.Name));
            }

            if (movie.Casts != null)
            {
                model.Casts = movie.Casts.Select(c => c.Id).ToArray();

                model.CastToString = String.Join(", ", movie.Casts.Select(c => c.Name));
            }

            if (movie.Directors != null)
            {
                model.Directors = movie.Directors.Select(d => d.Id).ToArray();

                model.DirectorToString = String.Join(", ", movie.Directors.Select(d => d.Name));
            }

            if (movie.Country != null)
            {
                model.Country = movie.Country.Id;
            }

            if (movie.Videos != null)
            {
                model.Videos = new string[movie.EpisodeCount];

                for (int i = 0; i < movie.EpisodeCount; i++)
                {
                    Video? video = movie.Videos.FirstOrDefault(m => m.Episode == i + 1);

                    if (video != null)
                        model.Videos[i] = video.VideoUrl ?? "";
                    else
                        model.Videos[i] = "";
                }
            }

            return model;
        }
	}
}
