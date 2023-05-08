using Microsoft.EntityFrameworkCore;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.Repositories
{
	public class CommentRepository : Repository<Comment>, ICommentRepository
	{
#nullable disable
        public CommentRepository(PhimStrongDbContext context) : base(context) {}

        public async Task<PagedList<Comment>> GetByMovieIdAsync(string movieId, PagingParameter pagingParameter)
        {
            IQueryable<Comment> comments = this._context.Comments
                                      .Include(c => c.Movie)
                                      .Include(c => c.User)
                                      .Include(c => c.ResponseTo)
                                      .Include(c => c.Responses)
                                      .ThenInclude(c => c.User)
									  .Where(c => c.Movie.Id == movieId && c.ResponseTo == null)
                                      .OrderByDescending(c => c.CreatedAt);

            return await PagedList<Comment>.ToPagedList(comments, pagingParameter.Page, pagingParameter.Size);
        }
    }
}
