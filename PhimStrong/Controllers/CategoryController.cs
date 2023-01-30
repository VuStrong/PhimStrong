using Microsoft.AspNetCore.Mvc;
using PhimStrong.Data;
using SharedLibrary.Constants;
using SharedLibrary.Helpers;
using SharedLibrary.Models;

namespace PhimStrong.Controllers
{
	public class CategoryController : Controller
	{
		private readonly AppDbContext _db;

		public CategoryController(AppDbContext db)
		{
			_db = db;
		}

		[HttpGet]
		[Route("/Category/{value}")]
		public IActionResult Index(string? value, int page)
		{
			if (value == null) value = "";

			value = value.RemoveMarks();

			Category? category = _db.Categories.FirstOrDefault(c => c.NormalizeName == value);

			if (category == null)
			{
				return NotFound("Không tìm thấy thể loại " + value);
			}

			List<Movie> movies = category.Movies != null ?
				category.Movies.OrderByDescending(m => m.ReleaseDate).ToList() :
				new List<Movie>();

			int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
			if (page > numberOfPages) page = numberOfPages;
			if (page <= 0) page = 1;

			movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
				.Take(CommonConstants.MOVIES_PER_PAGE).ToList();

			TempData["NumberOfPages"] = numberOfPages;
			TempData["CurrentPage"] = page;
			TempData["RouteValue"] = value;

			TempData["Title"] = category.Name;
			ViewData["Filter"] = "Thể loại";
			ViewData["Description"] = category.Description;

			return View(movies);
		}
	}
}
