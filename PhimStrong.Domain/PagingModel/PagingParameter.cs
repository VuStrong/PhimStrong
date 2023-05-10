namespace PhimStrong.Domain.PagingModel
{
	public class PagingParameter
	{
		public int Page { get; set; }
		public int Size { get; set; }
		public bool AllowCalculateCount { get; set; }

		public PagingParameter(int page, int size, bool allowCalculateCount = true)
		{
			Page = page;
			Size = size;
			AllowCalculateCount = allowCalculateCount;
		}
	}
}
