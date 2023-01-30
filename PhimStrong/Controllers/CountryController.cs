using Microsoft.AspNetCore.Mvc;
using PhimStrong.Data;
using SharedLibrary.Constants;
using SharedLibrary.Helpers;
using SharedLibrary.Models;

namespace PhimStrong.Controllers
{
	public class CountryController : Controller
	{
		private readonly AppDbContext _db;

		public CountryController(AppDbContext db)
		{
			_db = db;
		}

		[HttpGet]
		[Route("/Country/{value}")]
		public IActionResult Index(string? value, int page)
		{
			if (value == null) value = "";

			value = value.RemoveMarks();

			Country? country = _db.Countries.FirstOrDefault(c => c.NormalizeName == value);

			if (country == null)
			{
				return NotFound("Không tìm thấy quốc gia " + value);
			}

			List<Movie> movies = country.Movies != null ?
				country.Movies.OrderByDescending(m => m.ReleaseDate).ToList() :
				new List<Movie>();

			int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
			if (page > numberOfPages) page = numberOfPages;
			if (page <= 0) page = 1;

			movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
				.Take(CommonConstants.MOVIES_PER_PAGE).ToList();

			TempData["NumberOfPages"] = numberOfPages;
			TempData["CurrentPage"] = page;
			TempData["RouteValue"] = value;

			TempData["Title"] = country.Name;
			ViewData["Filter"] = "Quốc gia";
			ViewData["Description"] = country.About;

			return View(movies);
		}
	}
}
