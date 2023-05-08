using PhimStrong.Domain.Models;

namespace PhimStrong.Domain.Interfaces
{
	public interface ICastRepository : IRepository<Cast>
	{
		Task<int> MaxIdNumberAsync();
	}
}
