namespace PhimStrong.Domain.Exceptions.NotFound
{
    public sealed class MovieNotFoundException : NotFoundException
    {
        public MovieNotFoundException(string id) : base($"Movie with id '{id}' was not found.") { }
    }
}
