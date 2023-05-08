using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Category;
using PhimStrong.Models.Movie;

namespace PhimStrong.Controllers
{
	public class CategoryController : Controller
	{
		private const int MOVIES_PER_PAGE = 25;

		private readonly IMapper _mapper;
		private readonly ICategoryService _categoryService;
		private readonly IMovieService _movieService;

		public CategoryController(ICategoryService categoryService, IMovieService movieService, IMapper mapper)
		{
			_categoryService = categoryService;
			_movieService = movieService;
			_mapper = mapper;
		}

		[HttpGet]
		[Route("/Category/{value}")]
		public async Task<IActionResult> Index(string? value, int page)
		{
			Category? category = await _categoryService.GetByNameAsync(value ?? "");

			if (category == null)
			{
				return NotFound("Không tìm thấy thể loại " + value);
			}

			PagedList<Movie> movies = await _movieService.FindByCategoryIdAsync(
															category.Id,
															new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["RouteValue"] = value;

			CategoryViewModel model = _mapper.Map<CategoryViewModel>(category);
			model.Movies = _mapper.Map<PagedList<MovieViewModel>>(movies);

			return View(model);
		}
	}
}
