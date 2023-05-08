using Microsoft.EntityFrameworkCore;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.Repositories
{
	public class CountryRepository : Repository<Country>, ICountryRepository
	{
		public CountryRepository(PhimStrongDbContext context) : base(context) {}

		public async Task<int> MaxIdNumberAsync()
		{
			return await this._context.Countries.MaxAsync(c => c.IdNumber);
		}
	}
}
