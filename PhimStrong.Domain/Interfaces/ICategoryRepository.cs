using PhimStrong.Domain.Models;

namespace PhimStrong.Domain.Interfaces
{
	public interface ICategoryRepository : IRepository<Category>
	{
		Task<int> MaxIdNumberAsync();
	}
}
