using Microsoft.EntityFrameworkCore;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.Repositories
{
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		public CategoryRepository(PhimStrongDbContext context) : base(context) {}

		public async Task<int> MaxIdNumberAsync()
		{
			return await this._context.Categories.MaxAsync(c => c.IdNumber);
		}
	}
}
