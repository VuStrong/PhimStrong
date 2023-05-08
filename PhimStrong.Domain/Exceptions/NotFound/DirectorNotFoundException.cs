namespace PhimStrong.Domain.Exceptions.NotFound
{
    public sealed class DirectorNotFoundException : NotFoundException
    {
        public DirectorNotFoundException(string id) : base($"Director with id '{id}' was not found.") { }
    }
}
