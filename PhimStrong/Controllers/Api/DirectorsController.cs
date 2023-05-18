using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using PhimStrong.Resources.Director;
using PhimStrong.Resources.Movie;

namespace PhimStrong.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class DirectorsController : ControllerBase
	{
		private readonly IMovieService _movieService;
		private readonly IDirectorService _directorService;
		private readonly IMapper _mapper;

		public DirectorsController(
			IMovieService movieService,
			IDirectorService directorService, 
			IMapper mapper)
		{
			_movieService = movieService;
			_directorService = directorService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsync([FromQuery(Name = "q")] string? value, [FromQuery]PagingParameter pagingParameter)
		{
			PagedList<Director> directors = await _directorService.SearchAsync(value, pagingParameter);

			return Ok(_mapper.Map<PagedList<DirectorResource>>(directors).GetMetaData());
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAsync(string id)
		{
			Director? director = await _directorService.GetByIdAsync(id);

			if (director == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<DirectorResource>(director));
		}

		[HttpGet("{id}/movies")]
		public async Task<IActionResult> GetMovies(string id, [FromQuery]PagingParameter pagingParameter)
		{
			PagedList<Movie> movies = await _movieService.FindByDirectorIdAsync(id, pagingParameter);

			return Ok(_mapper.Map<PagedList<MovieResource>>(movies).GetMetaData());
		}
	}
}
