using Microsoft.AspNetCore.Mvc;
using PhimStrong.Data;
using SharedLibrary.Constants;
using SharedLibrary.Helpers;
using SharedLibrary.Models;

namespace PhimStrong.Controllers
{
	public class DirectorController : Controller
	{
		private readonly AppDbContext _db;

		public DirectorController(AppDbContext db)
		{
			_db = db;
		}

		[HttpGet]
		[Route("/Director/{value}")]
		public IActionResult Index(string? value, int page)
		{
			if (value == null) value = "";

			value = value.RemoveMarks();

			Director? director = _db.Directors.FirstOrDefault(d => d.NormalizeName == value);

			if (director == null)
			{
				return NotFound("Không tìm thấy đạo diễn " + value);
			}

			List<Movie> movies = director.Movies != null ?
				director.Movies.OrderByDescending(m => m.ReleaseDate).ToList() :
				new List<Movie>();

			int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
			if (page > numberOfPages) page = numberOfPages;
			if (page <= 0) page = 1;

			movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
				.Take(CommonConstants.MOVIES_PER_PAGE).ToList();

			TempData["NumberOfPages"] = numberOfPages;
			TempData["CurrentPage"] = page;
			TempData["RouteValue"] = value;

			TempData["Title"] = director.Name;
			ViewData["Filter"] = "Đạo diễn";
			ViewData["Description"] = director.About;
			ViewData["Image"] = director.Avatar;

			return View(movies);
		}
	}
}
