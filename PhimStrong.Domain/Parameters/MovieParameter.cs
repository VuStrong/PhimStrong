namespace PhimStrong.Domain.Parameters
{
	public class MovieParameter : PagingParameter
	{
#pragma warning disable
		public string? Value { get; set; }
		public string? Type { get; set; }
		public string? Status { get; set; }
		public string? Tag { get; set; }
		public string? OrderBy { get; set; }
		public int Year { get; set; }
		public int BeforeYear { get; set; }

		public MovieParameter() { }

		public MovieParameter(int page, int size, bool allowCalculateCount = true) : base(page, size, allowCalculateCount)
		{ }
	}
}
