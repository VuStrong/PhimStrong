using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhimStrong.Data;
using SharedLibrary.Models;
using SharedLibrary.Constants;
using PhimStrong.Models;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Helpers;
using NuGet.Packaging;

namespace PhimStrong.Controllers
{
    public class MovieController : Controller
    {
		private readonly AppDbContext _db;
		private readonly UserManager<User> _userManager;

		public MovieController(AppDbContext db, UserManager<User> userManager)
		{
			_db = db;
			_userManager = userManager;
		}

		[HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            List<Movie> movies = await _db.Movies.OrderByDescending(m => m.CreatedDate).ToListAsync();

			int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
            if (page > numberOfPages) page = numberOfPages;
			if (page <= 0) page = 1;

			movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
	            .Take(CommonConstants.MOVIES_PER_PAGE).ToList();

			TempData["NumberOfPages"] = numberOfPages;
			TempData["CurrentPage"] = page;

			TempData["Title"] = "Danh sách phim";
            ViewData["Filter"] = "Phim";

            return View(movies);
        }

        [HttpGet]
        [Route("/Movie/{value}")]
        public async Task<IActionResult> SearchByMovieName(string? value, int page)
        {
            if (value == null) value = "";

            TempData["Title"] = value;
            ViewData["Filter"] = "Phim có tên";

            value = value.RemoveMarks();

            List<Movie> movies = await _db.Movies.Where(m => 
					(m.NormalizeTranslateName ?? "").Contains(value) ||
					(m.NormalizeName ?? "").Contains(value)
				)
                .OrderByDescending(m => m.ReleaseDate).ToListAsync();

			int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
			if (page > numberOfPages) page = numberOfPages;
			if (page <= 0) page = 1;

            movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
                .Take(CommonConstants.MOVIES_PER_PAGE).ToList();                      

            TempData["NumberOfPages"] = numberOfPages;
            TempData["CurrentPage"] = page;
            TempData["Action"] = "SearchByMovieName";
            TempData["RouteValue"] = value;

			return View("Index", movies);
        }

		[HttpGet]
		[Route("/Movie/phim-le")]
		public async Task<IActionResult> GetPhimLe(int page)
		{
			List<Movie> movies = await _db.Movies.Where(m => m.Type == "Phim lẻ")
				.OrderByDescending(m => m.CreatedDate).ToListAsync();

			int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
			if (page > numberOfPages) page = numberOfPages;
			if (page <= 0) page = 1;

			movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
				.Take(CommonConstants.MOVIES_PER_PAGE).ToList();

			TempData["NumberOfPages"] = numberOfPages;
			TempData["CurrentPage"] = page;
			TempData["Action"] = "GetPhimLe";

			TempData["Title"] = "Phim lẻ";
			ViewData["Filter"] = "Phim";

			return View("Index", movies);
		}

		[HttpGet]
		[Route("/Movie/phim-bo")]
		public async Task<IActionResult> GetPhimBo(int page)
		{
			List<Movie> movies = await _db.Movies.Where(m => m.Type == "Phim bộ")
				.OrderByDescending(m => m.CreatedDate).ToListAsync();

			int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
			if (page > numberOfPages) page = numberOfPages;
			if (page <= 0) page = 1;

			movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
				.Take(CommonConstants.MOVIES_PER_PAGE).ToList();

			TempData["NumberOfPages"] = numberOfPages;
			TempData["CurrentPage"] = page;
			TempData["Action"] = "GetPhimBo";

			TempData["Title"] = "Phim bộ";
			ViewData["Filter"] = "Phim";

			return View("Index", movies);
		}

        [HttpGet]
        [Route("/Movie/year/{year}")]
        public async Task<IActionResult> GetMovieByReleaseYear(int year, int page)
        {
            List<Movie> movies = await _db.Movies.Where(m =>
					m.ReleaseDate != null ? m.ReleaseDate.Value.Year == year : false
				)
                .OrderByDescending(m => m.ReleaseDate).ToListAsync();

            int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
            if (page > numberOfPages) page = numberOfPages;
            if (page <= 0) page = 1;

            movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
                .Take(CommonConstants.MOVIES_PER_PAGE).ToList();

            TempData["NumberOfPages"] = numberOfPages;
            TempData["CurrentPage"] = page;
            TempData["Action"] = "GetMovieByReleaseYear";

            TempData["Title"] = year.ToString();
            ViewData["Filter"] = "Phim ra mắt năm";

            return View("Index", movies);
        }

		[Route("/Movie/tag/{value}")]
		public async Task<IActionResult> GetMovieByTag(string? value, int page)
		{
			if (value == null) value = "";

			value = value.ToLower().Trim();

			List<Tag> tags = await _db.Tags.Where(t => t.TagName.ToLower() == value).ToListAsync();
			List<Movie> movies = tags.Select(t => t.Movie).ToList();

			int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
			if (page > numberOfPages) page = numberOfPages;
			if (page <= 0) page = 1;

			movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
				.Take(CommonConstants.MOVIES_PER_PAGE).ToList();

			TempData["NumberOfPages"] = numberOfPages;
			TempData["CurrentPage"] = page;
			TempData["Action"] = "GetMovieByReleaseYear";

			TempData["Title"] = value;
			ViewData["Filter"] = "Phim có Tag";

			return View("Index", movies);
		}

        [Route("/Movie/top-rating")]
        public async Task<IActionResult> GetTopRatingMovie(int page)
        {
            List<Movie> movies = await _db.Movies.OrderByDescending(m => m.Rating).ToListAsync();

            int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
            if (page > numberOfPages) page = numberOfPages;
            if (page <= 0) page = 1;

            movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
                .Take(CommonConstants.MOVIES_PER_PAGE).ToList();

            TempData["NumberOfPages"] = numberOfPages;
            TempData["CurrentPage"] = page;
            TempData["Action"] = "GetTopRatingMovie";

            TempData["Title"] = "Top Rating";
            ViewData["Filter"] = "Phim";

            return View("Index", movies);
        }

        [Route("/Movie/trailer")]
        public async Task<IActionResult> GetTrailer(int page)
		{
            List<Movie> movies = await _db.Movies.Where(m => m.Status == "Trailer")
				.OrderByDescending(m => m.ReleaseDate).ToListAsync();

            int numberOfPages = (int)Math.Ceiling((double)movies.Count / CommonConstants.MOVIES_PER_PAGE);
            if (page > numberOfPages) page = numberOfPages;
            if (page <= 0) page = 1;

            movies = movies.Skip((page - 1) * CommonConstants.MOVIES_PER_PAGE)
                .Take(CommonConstants.MOVIES_PER_PAGE).ToList();

            TempData["NumberOfPages"] = numberOfPages;
            TempData["CurrentPage"] = page;
            TempData["Action"] = "GetTrailer";

            TempData["Title"] = "";
            ViewData["Filter"] = "Trailer";

            return View("Index", movies);
        }

        [HttpGet]
		public async Task<IActionResult> Detail(int id)
		{
			Movie? movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);

			if (movie == null)
			{
				return NotFound("Không tìm thấy phim :((");
			}

			return View(movie);
		}

		[HttpGet]
		[Route("/Movie/GetRelateMovies")]
		public async Task<IActionResult> GetRelateMovies(int movieid)
		{
			Movie? movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieid);

			if (movie == null)
			{
				return this.PartialView("_RelatedMoviePartial");
			}

			List<Movie> movieList = _db.Movies.ToList();
			List<Movie> movies = new();

			// find movie with similar cast
			movies.AddRange(movie.Casts != null ?
				movieList.Where(m => !movies.Contains(m) && m.Casts != null && m.Casts.Any(c => movie.Casts.Contains(c)))
				.Take(5).ToList() : new List<Movie>());

			// find movie with similar category
			movies.AddRange(movie.Categories!= null ?
				movieList.Where(m => !movies.Contains(m) && m.Categories != null && m.Categories.Any(m1 => movie.Categories.Contains(m1)))
				.Take(5).ToList() : new List<Movie>());

			// if less than 10 movies, add more
			Random random = new();
			int ran;
			while (movies.Count < 10)
			{
				ran = random.Next(0, movieList.Count);
				if (!movies.Contains(movieList[ran]))
				{
					movies.Add(movieList[ran]);
				}
			}

			return this.PartialView("_RelatedMoviePartial", movies);
		}

		[HttpGet]
		[Route("/Movie/GetLikeButton")]
		public async Task<string> GetLikeButton(int movieid)
		{
			Movie? movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieid);

			if (movie == null)
			{
				return $"<a id='like-movie-btn' href=\"#\" title=\"thích\" class=\"btn btn-success movie-detail-btn\">\r\n\t\t\t\t<i class=\"bi bi-hand-thumbs-up-fill\"></i>\r\n\t\t\t\t<strong>Thích</strong>\r\n\t\t\t\t<span class=\"ms-1\">0</span>\r\n\t\t\t</a>";
			}
			
			User user = await _userManager.GetUserAsync(User);

			movie.LikedUsers ??= new List<User>();
			string like = movie.LikedUsers.Contains(user) ? "Đã thích" : "Thích";

			if (user == null)
			{
				return $"<a id='like-movie-btn' href=\"#\" title=\"thích\" class=\"btn btn-success movie-detail-btn\">\r\n\t\t\t\t<i class=\"bi bi-hand-thumbs-up-fill\"></i>\r\n\t\t\t\t<strong>Thích</strong>\r\n\t\t\t\t<span class=\"ms-1\">{movie.LikedUsers.Count}</span>\r\n\t\t\t</a>";
			}

			return $"<a id='like-movie-btn' href=\"#\" title=\"thích\" class=\"btn btn-success movie-detail-btn\">\r\n\t\t\t\t<i class=\"bi bi-hand-thumbs-up-fill\"></i>\r\n\t\t\t\t<strong>{like}</strong>\r\n\t\t\t\t<span class=\"ms-1\">{movie.LikedUsers.Count}</span>\r\n\t\t\t</a>";
		}

		[HttpPost]
		[Route("/Movie/LikeMovie")]
		public async Task<JsonResult> LikeMovie(int movieid)
		{
			Movie? movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieid);

            if (movie == null)
            {
                return Json(new { success = false });
            }

			User user = await _userManager.GetUserAsync(User);

			if(user == null)
			{
                return Json(new { success = false, notsignin = true });
            }

			movie.LikedUsers ??= new List<User>();

			bool like = true;
			if(movie.LikedUsers.Contains(user))
			{
				like = false;
				movie.LikedUsers.Remove(user);
			}	
			else
			{
                movie.LikedUsers.Add(user);
			}

            try
            {
				await _db.SaveChangesAsync();
			}
			catch
			{
                return Json(new { success = false });
            }

            return Json(new {success = true, like = like});
		}

		[HttpGet]
		[Route("/Movie/Watch/{id}/{episode?}")]
		public async Task<IActionResult> Watch(int id, int episode)
		{
			Movie? movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);

			if (movie == null)
			{
				return NotFound();
			}

			movie.Videos ??= new List<Video>();
			Video? video = movie.Videos.FirstOrDefault(v => v.Episode == episode);

			TempData["episode"] = episode;

			return View(new WatchMovieModel
			{
				Movie = movie,
				Video = video
			});
		}

		[HttpPost]
		[Route("/Movie/IncreaseView")]
		public async Task<JsonResult> IncreaseView(int id)
		{
			Movie? movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);

			if (movie == null)
			{
				return Json("");
			}

			movie.View++;
			try
			{
				_db.Movies.Update(movie);
				await _db.SaveChangesAsync();
			}
			catch
			{
				return Json("");
			}

			return Json("");
		}
	}
}
