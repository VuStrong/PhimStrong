using PhimStrong.Domain.Models;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.Repositories
{
	public class TagRepository : Repository<Tag>
	{
		public TagRepository(PhimStrongDbContext context) : base(context) {}
	}
}
