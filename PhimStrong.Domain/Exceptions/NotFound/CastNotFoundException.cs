namespace PhimStrong.Domain.Exceptions.NotFound
{
    public sealed class CastNotFoundException : NotFoundException
    {
        public CastNotFoundException(string id) : base($"Cast with id '{id}' was not found.") { }
    }
}
