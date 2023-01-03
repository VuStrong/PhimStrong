namespace PhimStrong.Models
{
#nullable disable
    public class PagingModel
    {
        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }

        public Func<int, string> Callback { get; set; }
    }
}
