using Microsoft.EntityFrameworkCore;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.Repositories
{
	public class DirectorRepository : Repository<Director>, IDirectorRepository
	{
		public DirectorRepository(PhimStrongDbContext context) : base(context) {}

		public async Task<int> MaxIdNumberAsync()
		{
			return await this._context.Directors.MaxAsync(d => d.IdNumber);
		}
	}
}
