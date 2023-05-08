using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Movie;

namespace PhimStrong.Models.Director
{
    public class DirectorViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? About { get; set; }

		public PagedList<MovieViewModel> Movies { get; set; }

		public DirectorViewModel(string id, string name, PagedList<MovieViewModel> movies)
        {
            Id = id;
            Name= name;
            Movies = movies;
        }
    }
}
