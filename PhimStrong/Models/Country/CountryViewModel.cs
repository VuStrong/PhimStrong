using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Movie;

namespace PhimStrong.Models.Country
{
    public class CountryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? About { get; set; }

		public PagedList<MovieViewModel> Movies { get; set; }

		public CountryViewModel(string id, string name, PagedList<MovieViewModel> movies)
        {
            Id = id;
            Name = name;
            Movies = movies;
        }
    }
}
