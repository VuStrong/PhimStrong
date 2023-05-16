namespace PhimStrong.Domain.Parameters
{
    public class PagingParameter
    {
        public int Page { get; set; }
        public int Size { get; set; } = 10;
        public bool AllowCalculateCount { get; set; } = true;

        public PagingParameter() { }

        public PagingParameter(int page, int size, bool allowCalculateCount = true)
        {
            Page = page;
            Size = size;
            AllowCalculateCount = allowCalculateCount;
        }
    }
}
