using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using System.Linq.Expressions;

namespace PhimStrong.Domain.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
	{
		Task<PagedList<Movie>> GetAsync(MovieParameter movieParameter);
		Task<PagedList<Movie>> GetMovieByTagNameAsync(string tagName, PagingParameter pagingParameter);
		Task<int> MaxIdNumberAsync();
	}
}
