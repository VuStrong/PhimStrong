﻿using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using System.Linq.Expressions;

namespace PhimStrong.Application.Interfaces
{
    public interface IMovieService
	{
		Task<PagedList<Movie>> GetAllAsync(PagingParameter pagingParameter);
		Task<PagedList<Movie>> SearchAsync(MovieParameter movieParameter);
		Task<PagedList<Movie>> FindByTypeAsync(string type, PagingParameter pagingParameter);
		Task<PagedList<Movie>> FindByYearAsync(int year, PagingParameter pagingParameter);
		Task<PagedList<Movie>> FindBeforeYearAsync(int year, PagingParameter pagingParameter);
		Task<PagedList<Movie>> FindByTagAsync(string tag, PagingParameter pagingParameter);

		Task<PagedList<Movie>> FindByCastIdAsync(
			string castid, 
			PagingParameter pagingParameter, 
			Expression<Func<Movie, object?>>[]? includes = null);
		Task<PagedList<Movie>> FindByCategoryIdAsync(
			string categoryid, 
			PagingParameter pagingParameter, 
			Expression<Func<Movie, object?>>[]? includes = null);
		Task<PagedList<Movie>> FindByDirectorIdAsync(
			string directorid, 
			PagingParameter pagingParameter, 
			Expression<Func<Movie, object?>>[]? includes = null);
		Task<PagedList<Movie>> FindByCountryIdAsync(
			string countryid, 
			PagingParameter pagingParameter, 
			Expression<Func<Movie, object?>>[]? includes = null);

		/// <summary>
		/// Get all movie with status = 'Trailer'
		/// </summary>
		Task<PagedList<Movie>> GetTrailerAsync(PagingParameter pagingParameter);

		Task<PagedList<Movie>> GetMoviesOrderByRatingAsync(PagingParameter pagingParameter);
		Task<IEnumerable<Movie>> GetRandomMoviesAsync(int count, Expression<Func<Movie, object?>>[]? includes = null);

		/// <summary>
		/// Get related movies
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<Movie>> GetRelateMoviesAsync(string movieid, int maxCount, Expression<Func<Movie, object?>>[]? includes = null);

		Task<Movie?> GetByIdAsync(string id, Expression<Func<Movie, object?>>[]? includes = null);

		Task<Movie> CreateAsync(
			Movie movie,
			string[]? casts = null,
			string[]? directors = null,
			string[]? categories = null,
			string? country = null,
			string? tags = null,
			string[]? videos = null);

        Task<Movie> UpdateAsync(
			string movieid,
			Movie movie,
			string[]? casts = null,
            string[]? directors = null,
            string[]? categories = null,
            string? country = null,
            string? tags = null,
            string[]? videos = null);

		Task DeleteAsync(string movieid);
		Task IncreateViewAsync(string movieid);
		Task<bool> LikeMovieAsync(string movieid, string userid);
	}
}
