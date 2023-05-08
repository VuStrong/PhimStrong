using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using System.Linq.Expressions;

namespace PhimStrong.Application.Services
{
	public class CommentService : ICommentService
	{
		private readonly IUnitOfWork _unitOfWork;

		public CommentService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

        public async Task CreateAsync(Comment comment)
        {
			_unitOfWork.CommentRepository.Create(comment);
			await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int commentid)
        {
            Comment? comment = await _unitOfWork.CommentRepository.FirstOrDefaultAsync(
                c => c.Id == commentid,
                new Expression<Func<Comment, object?>>[]
                {
                    c => c.Responses
                });

			if (comment == null) throw new Exception();

            if (comment.Responses != null && comment.Responses.Count > 0)
            {
                foreach (var cmt in comment.Responses)
                {
                    _unitOfWork.CommentRepository.Delete(cmt);
                }
            }

            _unitOfWork.CommentRepository.Delete(comment);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _unitOfWork.CommentRepository.FirstOrDefaultAsync(
				c => c.Id == id,
				new Expression<Func<Comment, object?>>[]
				{
					c => c.User
				});
        }

        public async Task<PagedList<Comment>> GetByMovieIdAsync(string movieid, PagingParameter pagingParameter)
		{
			return await _unitOfWork.CommentRepository.GetByMovieIdAsync(movieid, pagingParameter);
		}

        public async Task LikeComment(int commentid)
        {
            Comment? comment = await _unitOfWork.CommentRepository.FirstOrDefaultAsync(c => c.Id == commentid);

            if (comment == null) throw new Exception();

            comment.Like++;
            _unitOfWork.CommentRepository.Update(comment);
            await _unitOfWork.SaveAsync();
        }
    }
}
