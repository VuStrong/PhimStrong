using PhimStrong.Domain.Models;

namespace PhimStrong.Domain.Interfaces
{
	public interface ICountryRepository : IRepository<Country>
	{
		Task<int> MaxIdNumberAsync();
	}
}
