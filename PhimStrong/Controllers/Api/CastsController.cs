using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using PhimStrong.Resources.Cast;
using PhimStrong.Resources.Movie;

namespace PhimStrong.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class CastsController : ControllerBase
	{
		private readonly IMovieService _movieService;
		private readonly ICastService _castService;
		private readonly IMapper _mapper;

		public CastsController(
			IMovieService movieService,
			ICastService castService,
			IMapper mapper)
		{
			_movieService = movieService;
			_castService = castService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsync([FromQuery(Name = "q")]string? value, [FromQuery]PagingParameter pagingParameter)
		{
			PagedList<Cast> casts = await _castService.SearchAsync(value, pagingParameter);

			return Ok(_mapper.Map<PagedList<CastResource>>(casts).GetMetaData());
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAsync(string id)
		{
			Cast? cast = await _castService.GetByIdAsync(id);

			if (cast == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<CastDetailResource>(cast));
		}

		[HttpGet("{id}/movies")]
		public async Task<IActionResult> GetMovies(string id, [FromQuery] PagingParameter pagingParameter)
		{
			PagedList<Movie> movies = await _movieService.FindByCastIdAsync(id, pagingParameter);

			return Ok(_mapper.Map<PagedList<MovieResource>>(movies).GetMetaData());
		}
	}
}
