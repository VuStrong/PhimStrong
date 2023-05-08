using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Country;
using PhimStrong.Models.Movie;

namespace PhimStrong.Controllers
{
	public class CountryController : Controller
	{
		private const int MOVIES_PER_PAGE = 25;

		private readonly IMapper _mapper;
		private readonly ICountryService _countryService;
		private readonly IMovieService _movieService;

		public CountryController(ICountryService countryService, IMovieService movieService, IMapper mapper)
		{
			_countryService = countryService;
			_movieService = movieService;
			_mapper = mapper;
		}

		[HttpGet]
		[Route("/Country/{value}")]
		public async Task<IActionResult> Index(string? value, int page)
		{
			Country? country = await _countryService.GetByNameAsync(value ?? "");

			if (country == null)
			{
				return NotFound("Không tìm thấy quốc gia " + value);
			}

			PagedList<Movie> movies = await _movieService.FindByCountryIdAsync(
															country.Id,
															new PagingParameter(page, MOVIES_PER_PAGE));
			ViewData["RouteValue"] = value;

			CountryViewModel model = _mapper.Map<CountryViewModel>(country);
			model.Movies = _mapper.Map<PagedList<MovieViewModel>>(movies);

			return View(model);
		}
	}
}
