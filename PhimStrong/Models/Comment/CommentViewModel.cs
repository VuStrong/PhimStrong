using PhimStrong.Models.User;

namespace PhimStrong.Models.Comment
{
    public class CommentViewModel
    {
		public int Id { get; set; }
		public string MovieId { get; set; }
		public UserViewModel User { get; set; }
		public string? Content { get; set; }
		public DateTime? CreatedAt { get; set; }
		public int Like { get; set; }
		public CommentViewModel? ResponseTo { get; set; }
		public List<CommentViewModel>? Responses { get; set; }

		public CommentViewModel(string movieId, UserViewModel user)
		{
			MovieId = movieId;
			User = user;
		}
	}
}
