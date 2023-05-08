using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;

namespace PhimStrong.Application.Interfaces
{
    public interface ICastService
    {
		Task<IEnumerable<Cast>> GetAllAsync();
		Task<PagedList<Cast>> GetAllAsync(PagingParameter pagingParameter);
        Task<PagedList<Cast>> SearchAsync(string? value, PagingParameter pagingParameter);
		Task<Cast?> GetByIdAsync(string id);
		Task<Cast?> GetByNameAsync(string name);

		Task<Cast> CreateAsync(Cast cast);
		Task<Cast> UpdateAsync(string castid, Cast cast);
        Task DeleteAsync(string castid);
    }
}
