using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;

namespace PhimStrong.Domain.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<PagedList<Comment>> GetByMovieIdAsync(string movieId, PagingParameter pagingParameter);
    }
}
