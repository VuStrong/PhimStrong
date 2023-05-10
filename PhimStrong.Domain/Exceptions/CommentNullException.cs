namespace PhimStrong.Domain.Exceptions
{
	public class CommentNullException : Exception
	{
		public CommentNullException() : base("Comment's movie and user is null") { }
	}
}
