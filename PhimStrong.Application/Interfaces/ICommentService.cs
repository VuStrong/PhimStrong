using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;

namespace PhimStrong.Application.Interfaces
{
	public interface ICommentService
	{
		Task<Comment?> GetByIdAsync(int id);
		Task<PagedList<Comment>> GetByMovieIdAsync(string movieid, PagingParameter pagingParameter);
		Task CreateAsync(Comment comment);
		Task DeleteAsync(int commentid);
		Task LikeComment(int commentid);
    }
}
