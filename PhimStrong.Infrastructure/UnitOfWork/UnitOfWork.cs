using PhimStrong.Infrastructure.Repositories;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly PhimStrongDbContext _context;

		public UnitOfWork(PhimStrongDbContext context)
		{
			_context = context;
		}

		private IMovieRepository? _movieRepository;
		public IMovieRepository MovieRepository
		{
			get
			{
				_movieRepository ??= new MovieRepository(_context);

				return _movieRepository;
			}
		}

        private ICastRepository? _castRepository;
        public ICastRepository CastRepository
        {
            get
            {
                _castRepository ??= new CastRepository(_context);

                return _castRepository;
            }
        }

		private ICategoryRepository? _categoryRepository;
		public ICategoryRepository CategoryRepository
		{
			get
			{
				_categoryRepository ??= new CategoryRepository(_context);

				return _categoryRepository;
			}
		}

		private IDirectorRepository? _directorRepository;
		public IDirectorRepository DirectorRepository
		{
			get
			{
				_directorRepository ??= new DirectorRepository(_context);

				return _directorRepository;
			}
		}

		private ICountryRepository? _countryRepository;
		public ICountryRepository CountryRepository
		{
			get
			{
				_countryRepository ??= new CountryRepository(_context);

				return _countryRepository;
			}
		}

		private IRepository<Tag>? _tagRepository;
		public IRepository<Tag> TagRepository 
		{
			get
			{
				_tagRepository ??= new TagRepository(_context);

				return _tagRepository;
			}
		}

		private IRepository<Video>? _videoRepository;
		public IRepository<Video> VideoRepository
		{
			get
			{
				_videoRepository ??= new VideoRepository(_context);

				return _videoRepository;
			}
		}

        private IRepository<User>? _userRepository;
        public IRepository<User> UserRepository
        {
            get
            {
                _userRepository ??= new UserRepository(_context);

                return _userRepository;
            }
        }

        private ICommentRepository? _commentRepository;
        public ICommentRepository CommentRepository
        {
            get
            {
                _commentRepository ??= new CommentRepository(_context);

                return _commentRepository;
            }
        }

        public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
