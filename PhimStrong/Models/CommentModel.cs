using SharedLibrary.Models;

namespace PhimStrong.Models
{
#pragma warning disable
	public class CommentModel
	{
		public int CommentCount { get; set; }
		public bool RenderCommentOnly { get; set; }
		public string? UserAvatar { get; set; }
		public bool UserLogin { get; set; }
		public int MovieId { get; set; }

		public List<Comment>? Comments { get; set; }
	}
}
