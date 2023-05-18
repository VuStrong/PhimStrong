using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using PhimStrong.Resources.Cast;
using PhimStrong.Resources.Category;
using PhimStrong.Resources.Director;
using PhimStrong.Resources.Movie;
using System.Linq.Expressions;
#pragma warning disable

namespace PhimStrong.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class MoviesController : ControllerBase
	{
		private readonly IMovieService _movieService;
		private readonly IMapper _mapper;

		public MoviesController(IMovieService movieService, IMapper mapper)
		{
			_movieService = movieService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsync([FromQuery]MovieParameterResource movieParameterResource)
		{
			MovieParameter movieParameter = _mapper.Map<MovieParameter>(movieParameterResource);

			PagedList<Movie> movies = await _movieService.SearchAsync(movieParameter);

			return Ok(_mapper.Map<PagedList<MovieResource>>(movies).GetMetaData());
		}
		
		[HttpGet("random")]
		public async Task<IActionResult> GetRandomMovies(int count = 10)
		{
			IEnumerable<Movie> randomMovies = await _movieService.GetRandomMoviesAsync(count);

			return Ok(_mapper.Map<IEnumerable<MovieResource>>(randomMovies));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAsync(string id)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Casts,
				m => m.Categories,
				m => m.Country,
				m => m.Directors,
				m => m.Tags,
			});

			if (movie == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<MovieDetailResource>(movie));
		}

		[HttpGet("{id}/related")]
		public async Task<IActionResult> GetRelatedMovies(string id, int count = 10)
		{
			IEnumerable<Movie> relatedMovies = await _movieService.GetRelateMoviesAsync(id, count);

			return Ok(_mapper.Map<IEnumerable<MovieResource>>(relatedMovies));
		}

		[HttpGet("{id}/videos")]
		public async Task<IActionResult> GetVideos(string id)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Videos.OrderBy(x => x.Episode),
			});

			if (movie == null)
			{
				return NotFound(nameof(movie));
			}

			return Ok(_mapper.Map<List<VideoResource>>(movie.Videos));
		}

		[HttpGet("{id}/videos/{episode}")]
		public async Task<IActionResult> GetVideo(string id, int episode)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Videos,
			});

			if (movie == null)
			{
				return NotFound(nameof(movie));
			}

			Video? video = movie.Videos.FirstOrDefault(v => v.Episode == episode);

			if (video == null)
			{
				return NotFound(nameof(video));
			}

			return Ok(_mapper.Map<VideoResource>(video));
		}

		[HttpGet("{id}/casts")]
		public async Task<IActionResult> GetCasts(string id)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Casts
			});

			if (movie == null)
			{
				return NotFound(nameof(movie));
			}

			return Ok(_mapper.Map<IEnumerable<CastResource>>(movie.Casts));
		}

		[HttpGet("{id}/categories")]
		public async Task<IActionResult> GetCategories(string id)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Categories
			});

			if (movie == null)
			{
				return NotFound(nameof(movie));
			}

			return Ok(_mapper.Map<IEnumerable<CategoryResource>>(movie.Categories));
		}

		[HttpGet("{id}/directors")]
		public async Task<IActionResult> GetDirectors(string id)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Directors
			});

			if (movie == null)
			{
				return NotFound(nameof(movie));
			}

			return Ok(_mapper.Map<IEnumerable<DirectorResource>>(movie.Directors));
		}

		[HttpGet("{id}/tags")]
		public async Task<IActionResult> GetTags(string id)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Tags
			});

			if (movie == null)
			{
				return NotFound(nameof(movie));
			}

			return Ok(movie.Tags.Select(t => t.TagName));
		}
	}
}
