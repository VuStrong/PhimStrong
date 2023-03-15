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

			Director? director = _db.Directors.ToList().FirstOrDefault(d => d.NormalizeName == value);

			if (director == null)
			{
				return NotFound("Không tìm thấy đạo diễn " + value);
			}

			director.Movies = director.Movies != null ?
				director.Movies.OrderByDescending(m => m.ReleaseDate).ToList() :
				new List<Movie>();

			int numberOfPages = (int)Math.Ceiling((double)director.Movies.Count / CommonConstants.MOVIES_PER_PAGE);
			if (page > numberOfPages) page = numberOfPages;
			if (page <= 0) page = 1;

			director.Movies = director.Movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
				.Take(CommonConstants.MOVIES_PER_PAGE).ToList();

			TempData["NumberOfPages"] = numberOfPages;
			TempData["CurrentPage"] = page;
			TempData["RouteValue"] = value;

			return View(director);
		}
	}
}
