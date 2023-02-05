using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Data;
using PhimStrong.Models;
using SharedLibrary.Models;
using System.Diagnostics;

namespace PhimStrong.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<Movie> randomMovies = new();
            Movie[] movies = _db.Movies.ToArray();
            int count = movies.Length;

            Random random = new();
            int randomNum;
            if (count > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i >= count) break;
                    randomNum = random.Next(0, count);

                    while (randomMovies.Contains(movies[randomNum]))
                    {
                        randomNum = random.Next(0, count);
                    }

                    randomMovies.Add(movies[randomNum]);
                }
            }

            ViewData["MoviesSlide"] = randomMovies;
            ViewData["ListMovieNew"] = _db.Movies.OrderByDescending(m => m.CreatedDate).Take(12).ToArray();
            ViewData["ListMovieTopRating"] = _db.Movies.OrderByDescending(m => m.Rating).Take(12).ToArray();
            ViewData["ListPhimLe"] = _db.Movies.Where(m => m.Type == "Phim lẻ").OrderByDescending(m => m.CreatedDate).Take(12).ToArray();
            ViewData["ListPhimBo"] = _db.Movies.Where(m => m.Type == "Phim bộ").OrderByDescending(m => m.CreatedDate).Take(12).ToArray();

            return View();
        }

        [Route("/chinh-sach-rieng-tu")]
        public IActionResult Privacy()
        {
			return View();
        }

        [Route("/dieu-khoan-su-dung")]
        public IActionResult TermsOfUse()
        {
            return View();
        }

        [Route("/khieu-nai-ban-quyen")]
        public IActionResult License()
        {
            return View();
        }

        [Route("/contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}