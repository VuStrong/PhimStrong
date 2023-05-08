using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;

namespace PhimStrong.Application.Interfaces
{
    public interface IDirectorService
	{
        Task<IEnumerable<Director>> GetAllAsync();
        Task<PagedList<Director>> GetAllAsync(PagingParameter pagingParameter);
        Task<PagedList<Director>> SearchAsync(string? value, PagingParameter pagingParameter);
        Task<Director?> GetByIdAsync(string id);
        Task<Director?> GetByNameAsync(string name);

        Task<Director> CreateAsync(Director director);
        Task<Director> UpdateAsync(string directorid, Director director);
        Task DeleteAsync(string directorid);
    }
}
