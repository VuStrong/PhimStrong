namespace PhimStrong.Domain.PagingModel
{
	public class PagingParameter
	{
		public int Page { get; set; }
		public int Size { get; set; }

		public PagingParameter(int page, int size)
		{
			Page = page;
			Size = size;
		}
	}
}
