using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Exceptions.NotFound;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using SharedLibrary.Helpers;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace PhimStrong.Application.Services
{
    public class MovieService : IMovieService
	{
		private readonly IUnitOfWork _unitOfWork;

		public MovieService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

        public async Task<Movie> CreateAsync(
            Movie movie,
			string[]? casts = null,
			string[]? directors = null,
			string[]? categories = null,
            string? country = null,
            string? tags = null,
			string[]? videos = null)
		{
			movie.IdNumber = await _unitOfWork.MovieRepository.AnyAsync() ?
				await _unitOfWork.MovieRepository.MaxIdNumberAsync() + 1 : 1;

			movie.Id = "ps" + movie.IdNumber.ToString();
			//Movie movie = new()
			//{
			//    IdNumber = idToCreate,
			//    Id = "ps" + idToCreate.ToString(),
			//    Name = model.Name.NormalizeString(),
			//    TranslateName = model.TranslateName.NormalizeString(),
			//    Description = model.Description,
			//    ReleaseDate = model.ReleaseDate,
			//    CreatedDate = DateTime.Now,
			//    ShowInSlide = false,
			//    Length = model.Length == null ? 0 : model.Length,
			//    Type = model.Type,
			//    EpisodeCount = model.EpisodeCount,
			//    Rating = model.Rating,
			//    Status = model.Status,
			//    Trailer = model.Trailer
			//};

			movie.Name = movie.Name.NormalizeString();
            movie.NormalizeName = movie.Name.RemoveMarks();
			movie.TranslateName = movie.TranslateName.NormalizeString();
            movie.NormalizeTranslateName = movie.TranslateName.RemoveMarks();
			movie.CreatedDate = DateTime.Now;

            if (categories != null && categories.Length > 0)
            {
                foreach (string cate in categories)
                {
                    Category? category = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(c => c.Name == cate);

                    if (category == null) continue;

                    movie.Categories ??= new List<Category>();

                    movie.Categories.Add(category);
                }
            }

            if (casts != null && casts.Length > 0)
            {
                foreach (string castName in casts)
                {

                    Cast? cast = await _unitOfWork.CastRepository.FirstOrDefaultAsync(c => c.Name == castName);

                    if (cast == null) continue;

                    movie.Casts ??= new List<Cast>();

                    movie.Casts.Add(cast);
                }
            }

            if (directors != null && directors.Length > 0)
            {
                foreach (string directorName in directors)
                {

                    Director? director = await _unitOfWork.DirectorRepository.FirstOrDefaultAsync(d => d.Name == directorName);

                    if (director == null) continue;

                    movie.Directors ??= new List<Director>();

                    movie.Directors.Add(director);
                }
            }

            if (!String.IsNullOrEmpty(country))
            {
                Country? countryToAdd = await _unitOfWork.CountryRepository.FirstOrDefaultAsync(c => c.Name == country);

                if (countryToAdd != null)
                {
                    movie.Country = countryToAdd;
                }
            }

            // videos
            if (videos != null && videos.Length > 0)
            {
                int index = 0;

                foreach (var v in videos)
                {
                    index += 1;
                    if (String.IsNullOrEmpty(v)) continue;

                    Video video = new()
                    {
                        VideoUrl = v,
                        Episode = index,
                        Movie = movie
                    };

                    movie.Videos ??= new List<Video>();
                    movie.Videos.Add(video);
                }
            }

            // tags
            if (tags != null)
            {
                string[] tagNames = new string[0];
                if (!String.IsNullOrEmpty(tags) && !String.IsNullOrWhiteSpace(tags))
                    tagNames = new Regex(@", |,").Split(tags);

                foreach (var tagName in tagNames)
                {
                    if (String.IsNullOrEmpty(tagName) || String.IsNullOrWhiteSpace(tagName)) continue;

                    Tag tag = new()
                    {
                        TagName = tagName.Trim(),
                        Movie = movie
                    };

                    movie.Tags ??= new List<Tag>();
                    movie.Tags.Add(tag);
                }
            }

			_unitOfWork.MovieRepository.Create(movie);
			await _unitOfWork.SaveAsync();
			return movie;
        }

		public async Task DeleteAsync(string movieid)
		{
			Movie? movie = await _unitOfWork.MovieRepository.FirstOrDefaultAsync(m => m.Id == movieid);

			if (movie == null) throw new MovieNotFoundException(movieid);

			_unitOfWork.MovieRepository.Delete(movie);
			await _unitOfWork.SaveAsync();
		}

		public async Task<PagedList<Movie>> GetAllAsync(PagingParameter pagingParameter)
		{
			return await _unitOfWork.MovieRepository.GetAsync(
														pagingParameter,
														orderBy: movies => movies.OrderByDescending(mv => mv.CreatedDate));
		}

		public async Task<Movie?> GetByIdAsync(string id, Expression<Func<Movie, object?>>[]? includes = null)
		{
			return await _unitOfWork.MovieRepository.FirstOrDefaultAsync(m => m.Id == id, includes);
		}

		public async Task<Movie> UpdateAsync(
            string movieid,
            Movie movie,
            string[]? casts = null,
            string[]? directors = null,
            string[]? categories = null,
            string? country = null,
            string? tags = null,
            string[]? videos = null)
		{
			Movie? movieToEdit = await _unitOfWork.MovieRepository.FirstOrDefaultAsync(
				m => m.Id == movieid,
				new Expression<Func<Movie, object?>>[]
				{
					m => m.Casts,
					m => m.Categories,
					m => m.Directors,
					m => m.Country,
					m => m.Tags,
					m => m.Videos,
                });

			if (movieToEdit == null) throw new MovieNotFoundException(movieid);

            movieToEdit.Name = movie.Name.NormalizeString();
            movieToEdit.NormalizeName = movieToEdit.Name.RemoveMarks();
            movieToEdit.TranslateName = movie.TranslateName.NormalizeString();
            movieToEdit.NormalizeTranslateName = movieToEdit.TranslateName.RemoveMarks();
            movieToEdit.Description = movie.Description;
            movieToEdit.ReleaseDate = movie.ReleaseDate;
            movieToEdit.Length = movie.Length;
            movieToEdit.Trailer = movie.Trailer;
            movieToEdit.Rating = movie.Rating;
            movieToEdit.Type = movie.Type;
            movieToEdit.Status = movie.Status;
            movieToEdit.EpisodeCount = movie.EpisodeCount;
			movieToEdit.Image = movie.Image;
			movieToEdit.CreatedDate = DateTime.Now;

            // Edit country
            if (movieToEdit.Country != null)
            {
                if (!String.IsNullOrEmpty(country) && country != movieToEdit.Country.Name)
                {
                    Country? countryToEdit = await _unitOfWork.CountryRepository.FirstOrDefaultAsync(c => c.Name == country);

                    if (countryToEdit != null)
                    {
                        movieToEdit.Country = countryToEdit;
                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(country))
                {
                    Country? countryToEdit = await _unitOfWork.CountryRepository.FirstOrDefaultAsync(c => c.Name == country);

                    if (countryToEdit != null)
                    {
                        movieToEdit.Country = countryToEdit;
                    }
                }
            }

            // Edit Category
            if (categories != null && categories.Length > 0)
            {
                if (movieToEdit.Categories != null)
                {
                    movieToEdit.Categories = null;
                }

                foreach (string cate in categories)
                {
                    Category? category = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(c => c.Name == cate);

                    if (category == null) continue;

                    movieToEdit.Categories ??= new List<Category>();

                    movieToEdit.Categories.Add(category);
                }
            }

            // Edit Cast
            if (casts != null && casts.Length > 0)
            {
                if (movieToEdit.Casts != null)
                {
                    movieToEdit.Casts = null;
                }

                foreach (string castName in casts)
                {
                    Cast? cast = await _unitOfWork.CastRepository.FirstOrDefaultAsync(c => c.Name == castName);

                    if (cast == null) continue;

                    movieToEdit.Casts ??= new List<Cast>();

                    movieToEdit.Casts.Add(cast);
                }
            }

            // Edit Director
            if (directors != null && directors.Length > 0)
            {
                if (movieToEdit.Directors != null)
                {
                    movieToEdit.Directors = null;
                }

                foreach (string directorName in directors)
                {
                    Director? director = await _unitOfWork.DirectorRepository.FirstOrDefaultAsync(d => d.Name == directorName);

                    if (director == null) continue;

                    movieToEdit.Directors ??= new List<Director>();

                    movieToEdit.Directors.Add(director);
                }
            }

            // Edit video
            if (videos != null && videos.Length > 0)
            {
                int index = 0;

                if (movieToEdit.Videos != null) movieToEdit.Videos = null;

                foreach (var v in videos)
                {
                    index += 1;
                    if (String.IsNullOrEmpty(v)) continue;

                    Video video = new()
                    {
                        VideoUrl = v,
                        Episode = index,
                        Movie = movieToEdit
                    };

                    movieToEdit.Videos ??= new List<Video>();
                    movieToEdit.Videos.Add(video);
                }
            }

            // Edit tag
            if (tags != null)
            {
                string[] tagNames = new string[0];
                if (!String.IsNullOrEmpty(tags) && !String.IsNullOrWhiteSpace(tags))
                    tagNames = new Regex(@", |,").Split(tags);

                if (movieToEdit.Tags != null) movieToEdit.Tags = null;

                foreach (string tagName in tagNames)
                {
                    if (String.IsNullOrEmpty(tagName) || String.IsNullOrWhiteSpace(tagName)) continue;

                    Tag tag = new()
                    {
                        TagName = tagName,
                        Movie = movieToEdit
                    };

                    movieToEdit.Tags ??= new List<Tag>();
                    movieToEdit.Tags.Add(tag);
                }
            }

			_unitOfWork.MovieRepository.Update(movieToEdit);
			await _unitOfWork.SaveAsync();
            return movieToEdit;
		}

		public async Task<PagedList<Movie>> SearchAsync(string? value, PagingParameter pagingParameter)
		{
			value = value?.RemoveMarks();

			PagedList<Movie> movies = value != null ?
				await _unitOfWork.MovieRepository.GetAsync(
														pagingParameter,
														m => (m.NormalizeTranslateName ?? "").Contains(value) ||
															 (m.NormalizeName ?? "").Contains(value),
														movies => movies.OrderByDescending(m => m.ReleaseDate)) :
				await this.GetAllAsync(pagingParameter);

			return movies;
		}

		public async Task<PagedList<Movie>> FindByTypeAsync(string type, PagingParameter pagingParameter)
		{
			return await _unitOfWork.MovieRepository.GetAsync(
														pagingParameter,
														m => m.Type == type,
														movies => movies.OrderByDescending(m => m.CreatedDate));
		}

		public async Task<PagedList<Movie>> FindByYearAsync(int year, PagingParameter pagingParameter)
		{
			return await _unitOfWork.MovieRepository.GetAsync(
														pagingParameter,
														m => m.ReleaseDate != null && m.ReleaseDate.Value.Year == year,
														movies => movies.OrderByDescending(m => m.ReleaseDate));
		}

		public async Task<PagedList<Movie>> FindBeforeYearAsync(int year, PagingParameter pagingParameter)
		{
			return await _unitOfWork.MovieRepository.GetAsync(
														pagingParameter,
														m => m.ReleaseDate != null && m.ReleaseDate.Value.Year <= year,
														movies => movies.OrderByDescending(m => m.ReleaseDate));
		}

		public async Task<PagedList<Movie>> FindByTagAsync(string tag, PagingParameter pagingParameter)
		{
			tag = tag.ToLower().Trim();

			return await _unitOfWork.MovieRepository.GetMovieByTagNameAsync(tag, pagingParameter);
		}

		/// <summary>
		/// Get all movie with status = 'Trailer'
		/// </summary>
		public async Task<PagedList<Movie>> GetTrailerAsync(PagingParameter pagingParameter)
		{
			return await _unitOfWork.MovieRepository.GetAsync(
														pagingParameter,
														m => m.Status == "Trailer",
														movies => movies.OrderByDescending(m => m.ReleaseDate));
		}

		public async Task<PagedList<Movie>> GetMoviesOrderByRatingAsync(PagingParameter pagingParameter)
		{
			return await _unitOfWork.MovieRepository.GetAsync(
														pagingParameter,
														orderBy: movies => movies.OrderByDescending(m => m.Rating));
		}

		public async Task<IEnumerable<Movie>> GetRelateMoviesAsync(string movieid, int maxCount)
		{
			Movie? movie = await this.GetByIdAsync(movieid, new Expression<Func<Movie, object?>>[]
			{
				m => m.Tags,
				m => m.Categories
			});

			if (movie == null)
			{
				return new List<Movie>();
			}

			List<Movie> movies = new();
			List<Movie> movieList = (await _unitOfWork.MovieRepository.GetAsync(
				includes: new Expression<Func<Movie, object?>>[]
				{
					m => m.Categories
				})).ToList();

			// find movie with similar tag
			if (movie.Tags != null && movie.Tags.Any())
			{
				string tagToFind = movie.Tags[0].TagName.ToLower();

				movies.AddRange(
					(await _unitOfWork.TagRepository.GetAsync(
						t => t.TagName.ToLower() == tagToFind,
						includes: new Expression<Func<Tag, object?>>[]
						{
							t => t.Movie
						}))
					.Select(t => t.Movie).ToList());

				if (movies.Contains(movie)) movies.Remove(movie);
			}

			// find movie with similar category
			if (movies.Count < maxCount && movie.Categories != null && movie.Categories.Any())
			{
				Category? cateToFind = movie.Categories[0];
				movies.AddRange(
					movieList.Where(m => !movies.Contains(m) &&
						!m.Equals(movie) &&
						m.Categories != null &&
						m.Categories.Any(c => c.Equals(cateToFind)))
					.Take(maxCount - movies.Count).ToList());
			}

			// if less than 10 movies, add more
			if (movies.Count < maxCount)
			{
				Random random = new();
				int ran;
				int count = movieList.Count;
				while (movies.Count < maxCount)
				{
					if (movies.Count >= count) break;

					ran = random.Next(0, count);
					if (!movies.Contains(movieList[ran]) && !movieList[ran].Equals(movie))
					{
						movies.Add(movieList[ran]);
					}
				}
			}

			return movies;
		}

		public async Task<IEnumerable<Movie>> GetRandomMoviesAsync(int count)
		{
			PagingParameter pagingParameter = new(1, count);

			return await _unitOfWork.MovieRepository.GetAsync(
														pagingParameter,
														orderBy: movies => movies.OrderBy(mv => Guid.NewGuid()));
		}

		public async Task IncreateViewAsync(string movieid)
		{
			Movie? movie = await _unitOfWork.MovieRepository.FirstOrDefaultAsync(m => m.Id == movieid);

			if (movie == null) return;

			movie.View++;

			_unitOfWork.MovieRepository.Update(movie);
			await _unitOfWork.SaveAsync();
		}

		public async Task<bool> AddLikedUserAsync(string movieid, User user)
		{
			Movie? movie = await _unitOfWork.MovieRepository.FirstOrDefaultAsync(
																m => m.Id == movieid,
																new Expression<Func<Movie, object?>>[]
																{
																	m => m.LikedUsers
																});

			if (movie == null)
			{
				throw new Exception();
			}

			movie.LikedUsers ??= new List<User>();
			if (movie.LikedUsers.Contains(user))
			{
				movie.LikedUsers.Remove(user);
				_unitOfWork.MovieRepository.Update(movie);
				await _unitOfWork.SaveAsync();

				return false;
			}
			else
			{
				movie.LikedUsers.Add(user);
				_unitOfWork.MovieRepository.Update(movie);
				await _unitOfWork.SaveAsync();

				return true;
			}
		}

		public async Task<PagedList<Movie>> FindByCastIdAsync(string castid, PagingParameter pagingParameter)
		{
			Cast? cast = await _unitOfWork.CastRepository.FirstOrDefaultAsync(
															c => c.Id == castid,
															new Expression<Func<Cast, object?>>[]
															{
																c => c.Movies
															});

			if (cast == null || cast.Movies == null) return new PagedList<Movie>();

			int total = cast.Movies.Count;
			List<Movie> result = cast.Movies.Skip((pagingParameter.Page - 1) * pagingParameter.Size)
											.Take(pagingParameter.Size).ToList();

			return new PagedList<Movie>(result, pagingParameter.Page, pagingParameter.Size, total);
		}

		public async Task<PagedList<Movie>> FindByCategoryIdAsync(string categoryid, PagingParameter pagingParameter)
		{
			Category? category = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(
															c => c.Id == categoryid,
															new Expression<Func<Category, object?>>[]
															{
																c => c.Movies
															});

			if (category == null || category.Movies == null) return new PagedList<Movie>();

			int total = category.Movies.Count;
			List<Movie> result = category.Movies.Skip((pagingParameter.Page - 1) * pagingParameter.Size)
											.Take(pagingParameter.Size).ToList();

			return new PagedList<Movie>(result, pagingParameter.Page, pagingParameter.Size, total);
		}

		public async Task<PagedList<Movie>> FindByDirectorIdAsync(string directorid, PagingParameter pagingParameter)
		{
			Director? director = await _unitOfWork.DirectorRepository.FirstOrDefaultAsync(
															d => d.Id == directorid,
															new Expression<Func<Director, object?>>[]
															{
																d => d.Movies
															});

			if (director == null || director.Movies == null) return new PagedList<Movie>();

			int total = director.Movies.Count;
			List<Movie> result = director.Movies.Skip((pagingParameter.Page - 1) * pagingParameter.Size)
											.Take(pagingParameter.Size).ToList();

			return new PagedList<Movie>(result, pagingParameter.Page, pagingParameter.Size, total);
		}

		public async Task<PagedList<Movie>> FindByCountryIdAsync(string countryid, PagingParameter pagingParameter)
		{
			Country? country = await _unitOfWork.CountryRepository.FirstOrDefaultAsync(
															c => c.Id == countryid,
															new Expression<Func<Country, object?>>[]
															{
																c => c.Movies
															});

			if (country == null || country.Movies == null) return new PagedList<Movie>();

			int total = country.Movies.Count;
			List<Movie> result = country.Movies.Skip((pagingParameter.Page - 1) * pagingParameter.Size)
											.Take(pagingParameter.Size).ToList();

			return new PagedList<Movie>(result, pagingParameter.Page, pagingParameter.Size, total);
		}
	}
}
