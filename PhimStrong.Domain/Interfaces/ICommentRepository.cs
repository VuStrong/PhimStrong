using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;

namespace PhimStrong.Domain.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<PagedList<Comment>> GetByMovieIdAsync(string movieId, PagingParameter pagingParameter);
    }
}
