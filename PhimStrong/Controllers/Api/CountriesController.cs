using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using PhimStrong.Resources.Country;
using PhimStrong.Resources.Movie;
using System.Linq.Expressions;

namespace PhimStrong.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController : ControllerBase
	{
		private readonly IMovieService _movieService;
		private readonly ICountryService _countryService;
		private readonly IMapper _mapper;

		public CountriesController(
			IMovieService movieService,
			ICountryService countryService,
			IMapper mapper)
		{
			_movieService = movieService;
			_countryService = countryService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsync()
		{
			IEnumerable<Country> countries = await _countryService.GetAllAsync();

			return Ok(_mapper.Map<IEnumerable<CountryResource>>(countries));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAsync(string id)
		{
			Country? country = await _countryService.GetByIdAsync(id);

			if (country == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<CountryResource>(country));
		}

		[HttpGet("{id}/movies")]
		public async Task<IActionResult> GetMovies(string id, [FromQuery] PagingParameter pagingParameter)
		{
			PagedList<Movie> movies = await _movieService.FindByCountryIdAsync(
				id, 
				pagingParameter,
				new Expression<Func<Movie, object?>>[]
				{
					m => m.Categories,
					m => m.Country
				});

			return Ok(_mapper.Map<PagedList<MovieResource>>(movies).GetMetaData());
		}
	}
}
