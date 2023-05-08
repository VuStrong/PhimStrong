using PhimStrong.Domain.Models;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.Repositories
{
	public class VideoRepository : Repository<Video>
	{
		public VideoRepository(PhimStrongDbContext context) : base(context) { }
	}
}
