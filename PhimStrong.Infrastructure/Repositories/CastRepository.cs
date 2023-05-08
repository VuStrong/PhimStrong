using Microsoft.EntityFrameworkCore;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.Repositories
{
	public class CastRepository : Repository<Cast>, ICastRepository
	{
		public CastRepository(PhimStrongDbContext context) : base(context) {}

		public async Task<int> MaxIdNumberAsync()
		{
			return await this._context.Casts.MaxAsync(c => c.IdNumber);
		}
	}
}
