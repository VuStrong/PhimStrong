using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;

namespace PhimStrong.Application.Interfaces
{
    public interface ICommentService
	{
		Task<Comment?> GetByIdAsync(int id);
		Task<PagedList<Comment>> GetByMovieIdAsync(string movieid, PagingParameter pagingParameter);
		Task CreateAsync(Comment comment, string? userid = null, string? movieid = null, int responseToId = 0);
		Task DeleteAsync(int commentid);
		Task LikeComment(int commentid);
    }
}
