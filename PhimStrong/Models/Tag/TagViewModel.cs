namespace PhimStrong.Models.Tag
{
	public class TagViewModel
	{
		public int Id { get; set; }
		public string TagName { get; set; }
		public string MovieId { get; set; }

		public TagViewModel(string tagName, string movieId)
		{
			TagName = tagName;
			MovieId = movieId;
		}
	}
}
