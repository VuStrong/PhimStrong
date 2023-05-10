using Microsoft.EntityFrameworkCore;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.Repositories
{
	public class MovieRepository : Repository<Movie>, IMovieRepository
	{
		public MovieRepository(PhimStrongDbContext context) : base(context) {}

		public async Task<PagedList<Movie>> GetMovieByTagNameAsync(string tagName, PagingParameter pagingParameter)
		{
			IQueryable<Movie> movies = this._context.Tags
													.Where(t => t.TagName.ToLower() == tagName)
													.Include(t => t.Movie)
													.Select(t => t.Movie);

			return await PagedList<Movie>.ToPagedList(
				movies, pagingParameter.Page, pagingParameter.Size, pagingParameter.AllowCalculateCount);
		}

		public async Task<int> MaxIdNumberAsync()
		{
			return await this._context.Movies.MaxAsync(m => m.IdNumber);
		}
	}
}
