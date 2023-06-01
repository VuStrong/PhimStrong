namespace PhimStrong.Models.Movie
{
	public class WatchMovieViewModel
	{
		public string MovieId { get; set; }
		public string MovieName { get; set; }
		public string MovieDescription { get; set; }
		public string MovieType { get; set; }
		public string? MovieImage { get; set; }
		public int MovieEpisodeCount { get; set; }
		public int Episode { get; set; }
		public string? VideoUrl { get; set; }

		public WatchMovieViewModel(string movieId, string movieName, string movieDescription, string movieType)
		{
			MovieId = movieId;
			MovieName = movieName;
			MovieDescription = movieDescription;
			MovieType = movieType;
		}
	}
}
