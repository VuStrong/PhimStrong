using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Movie;

namespace PhimStrong.Models.Category
{
    public class CategoryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

		public PagedList<MovieViewModel> Movies { get; set; }

		public CategoryViewModel(string id, string name, PagedList<MovieViewModel> movies)
        {
            Id = id;
            Name = name;
            Movies = movies;
        }
	}
}
