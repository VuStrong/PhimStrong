using PhimStrong.Domain.Models;

namespace PhimStrong.Domain.Interfaces
{
	public interface IUnitOfWork
	{
		IMovieRepository MovieRepository { get; }
		ICastRepository CastRepository { get; }
		ICategoryRepository CategoryRepository { get; }
		IDirectorRepository DirectorRepository { get; }
		ICountryRepository CountryRepository { get; }
		IRepository<Tag> TagRepository { get; }
		IRepository<Video> VideoRepository { get; }
        ICommentRepository CommentRepository { get; }
        Task SaveAsync();
	}
}
