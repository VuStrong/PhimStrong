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

        public string? Country { get; set; }

		[DisplayName("Đạo diễn")]
		public string[]? Directors { get; set; }
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

        [DisplayName("Diễn viên")]
		public string[]? Casts { get; set; }
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
                List<string> cateList = new();

                movie.Categories.ForEach((c) =>
                {
                    cateList.Add(c.Name);
                });

                model.Categories = cateList.ToArray();
            }

            if (movie.Casts != null)
            {
                List<string> castList = new();

                movie.Casts.ForEach((c) =>
                {
                    castList.Add(c.Name);
                });

                model.Casts = castList.ToArray();
            }

            if (movie.Directors != null)
            {
                List<string> directorList = new();

                movie.Directors.ForEach((d) =>
                {
                    directorList.Add(d.Name);
                });

                model.Directors = directorList.ToArray();
            }

            if (movie.Country != null)
            {
                model.Country = movie.Country.Name;
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
