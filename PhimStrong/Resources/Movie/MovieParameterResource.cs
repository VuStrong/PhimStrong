using Microsoft.AspNetCore.Mvc;

namespace PhimStrong.Resources.Movie
{
	public class MovieParameterResource
	{
		public int Page { get; set; }
		public int Size { get; set; } = 10;
		public bool AllowCalculateCount { get; set; } = true;
		[FromQuery(Name = "q")]
		public string? Value { get; set; }
		public string? Type { get; set; }
		public string? Status { get; set; }
		public string? Tag { get; set; }
		public string? OrderBy { get; set; }
		public string? Country { get; set; }
		public string[]? Categories { get; set; }
		public int Year { get; set; }
		public int BeforeYear { get; set; }
	}
}
