namespace PhimStrong.Domain.Exceptions.NotFound
{
    public abstract class NotFoundException : Exception
    {
        protected NotFoundException(string message) : base(message) { }
    }
}
