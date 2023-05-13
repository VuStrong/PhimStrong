namespace PhimStrong.Models.Comment
{
#pragma warning disable
    public class CreateCommentViewModel
    {
        public string MovieId { get; set; }
        public string Content { get; set; }
        public int ResponseToId { get; set; }
    }
}
