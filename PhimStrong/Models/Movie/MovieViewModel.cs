using PhimStrong.Models.Cast;
using PhimStrong.Models.Category;
using PhimStrong.Models.Country;
using PhimStrong.Models.Director;
using PhimStrong.Models.Tag;
using PhimStrong.Models.User;
using PhimStrong.Models.Video;

namespace PhimStrong.Models.Movie
{
    public class MovieViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TranslateName { get; set; }

        public string? Description { get; set; }
		public string[]? DescriptionSplit
		{
			get
			{
				if (this.Description != null)
				{
					return this.Description.Split("<br>");
				}
				else return null;
			}
		}

		public DateTime? ReleaseDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Length { get; set; }
        public int View { get; set; }
        public string? Image { get; set; }
        public string? Trailer { get; set; }
		public float Rating { get; set; }
		public string Type { get; set; }
		public int EpisodeCount { get; set; }

		public List<CategoryViewModel>? Categories { get; set; }
        public CountryViewModel? Country { get; set; }
        public List<DirectorViewModel>? Directors { get; set; }
        public List<CastViewModel>? Casts { get; set; }
        public List<VideoViewModel>? Videos { get; set; }
        public List<TagViewModel>? Tags { get; set; }

		public string? CategoryToString
        {
            get
            {
                if (Categories != null)
                {
                    return string.Join(",", Categories.Select(c => c.Name));
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
                if (Casts != null)
                {
                    return string.Join(",", Casts.Select(c => c.Name));
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
                if (Directors != null)
                {
                    return string.Join(",", Directors.Select(d => d.Name));
                }
                else
                {
                    return null;
                }
            }
        }

		public string Status { get; set; }
		public string StatusToString
		{
			get
			{
				if (this.Type == "Phim lẻ") return this.Status;
				else return "Tập " + this.EpisodeCount;
			}
		}

        public MovieViewModel(string id, string name, string translateName, string type, string status)
        {
            Id = id;
            Name = name;
            TranslateName = translateName;
            Type = type;
            Status = status;
        }
	}
}
