namespace PhimStrong.Domain.Exceptions.NotFound
{
    public sealed class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException(string id) : base($"Category with id '{id}' was not found.") { }
    }
}
