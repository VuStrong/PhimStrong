using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using PhimStrong.Models.Cast;
using PhimStrong.Models.Movie;

namespace PhimStrong.Controllers
{
	public class CastController : Controller
	{
		private const int MOVIES_PER_PAGE = 25;

		private readonly IMapper _mapper;
		private readonly ICastService _castService;
		private readonly IMovieService _movieService;

		public CastController(ICastService castService, IMovieService movieService, IMapper mapper)
		{
			_castService = castService;
			_movieService = movieService;
			_mapper = mapper;
		}

		[HttpGet]
		[Route("/Cast/{value}")]
		public async Task<IActionResult> Index(string? value, int page)
		{
			Cast? cast = await _castService.GetByNameAsync(value ?? "");

			if (cast == null)
			{
				return NotFound("Không tìm thấy diễn viên " + value);
			}

			PagedList<Movie> movies = await _movieService.FindByCastIdAsync(
														cast.Id,
														new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["RouteValue"] = value;

			CastViewModel model = _mapper.Map<CastViewModel>(cast);
			model.Movies = _mapper.Map<PagedList<MovieViewModel>>(movies);

			return View(model);
		}
	}
}
