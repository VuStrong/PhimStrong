namespace PhimStrong.Domain.Exceptions.NotFound
{
    public sealed class CountryNotFoundException : NotFoundException
    {
        public CountryNotFoundException(string id) : base($"Country with id '{id}' was not found.") { }
    }
}
