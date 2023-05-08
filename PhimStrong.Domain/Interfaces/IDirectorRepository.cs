using PhimStrong.Domain.Models;

namespace PhimStrong.Domain.Interfaces
{
	public interface IDirectorRepository : IRepository<Director>
	{
		Task<int> MaxIdNumberAsync();
	}
}
