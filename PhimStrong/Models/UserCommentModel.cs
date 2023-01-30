using SharedLibrary.Models;

namespace PhimStrong.Models
{
#pragma warning disable
	public class UserCommentModel
	{
		public int MovieId { get; set; }
		public string Content { get; set; }
		public int ResponseToId { get; set; }
	}
}
