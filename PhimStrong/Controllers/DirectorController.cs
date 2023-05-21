using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using PhimStrong.Models.Director;
using PhimStrong.Models.Movie;

namespace PhimStrong.Controllers
{
    [Route("[controller]")]
    public class DirectorController : Controller
	{
		private const int MOVIES_PER_PAGE = 25;

		private readonly IMapper _mapper;
		private readonly IDirectorService _directorService;
		private readonly IMovieService _movieService;

		public DirectorController(IDirectorService directorService, IMovieService movieService, IMapper mapper)
		{
			_directorService = directorService;
			_movieService = movieService;
			_mapper = mapper;
		}

		[HttpGet("{value}")]
		public async Task<IActionResult> Index(string? value, int page)
		{
			Director? director = await _directorService.GetByNameAsync(value ?? "");

			if (director == null)
			{
				return NotFound("Không tìm thấy đạo diễn " + value);
			}

			PagedList<Movie> movies = await _movieService.FindByDirectorIdAsync(
															director.Id,
															new PagingParameter(page, MOVIES_PER_PAGE));
			ViewData["RouteValue"] = value;

			DirectorViewModel model = _mapper.Map<DirectorViewModel>(director);
			model.Movies = _mapper.Map<PagedList<MovieViewModel>>(movies);

			return View(model);
		}
	}
}
