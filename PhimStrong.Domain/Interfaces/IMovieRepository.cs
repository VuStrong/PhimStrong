using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;

namespace PhimStrong.Domain.Interfaces
{
	public interface IMovieRepository : IRepository<Movie>
	{
		Task<PagedList<Movie>> GetMovieByTagNameAsync(string tagName, PagingParameter pagingParameter);
		Task<int> MaxIdNumberAsync();
	}
}
