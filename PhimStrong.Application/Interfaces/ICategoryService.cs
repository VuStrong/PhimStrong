using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;

namespace PhimStrong.Application.Interfaces
{
    public interface ICategoryService
	{
        Task<IEnumerable<Category>> GetAllAsync();
        Task<PagedList<Category>> GetAllAsync(PagingParameter pagingParameter);
        Task<PagedList<Category>> SearchAsync(string? value, PagingParameter pagingParameter);
        Task<Category?> GetByIdAsync(string id);
        Task<Category?> GetByNameAsync(string name);

        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(string categoryid, Category category);
        Task DeleteAsync(string categoryid);
    }
}
