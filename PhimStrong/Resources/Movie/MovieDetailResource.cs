using PhimStrong.Resources.Cast;
using PhimStrong.Resources.Category;
using PhimStrong.Resources.Country;
using PhimStrong.Resources.Director;

namespace PhimStrong.Resources.Movie
{
	public class MovieDetailResource
	{
#pragma warning disable
		public string Id { get; set; }
		public string Name { get; set; }
		public string TranslateName { get; set; }
		public string? Description { get; set; }
		public DateTime? ReleaseDate { get; set; }
		public DateTime? CreatedDate { get; set; }
		public int? Length { get; set; }
		public int View { get; set; }
		public string? Image { get; set; }

		public string? Trailer { get; set; }
		public float Rating { get; set; }
		public string Type { get; set; }
		public int EpisodeCount { get; set; }
		public string Status { get; set; }

		public IEnumerable<CastResource>? Casts { get; set; }
		public IEnumerable<DirectorResource>? Directors { get; set; }
		public IEnumerable<CategoryResource>? Categories { get; set; }
		public CountryResource? Country { get; set; }
		public IEnumerable<string>? Tags { get; set; }
	}
}
