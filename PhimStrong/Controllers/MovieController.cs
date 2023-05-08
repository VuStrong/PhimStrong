using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Models.Movie;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using AutoMapper;
using System.Linq.Expressions;

namespace PhimStrong.Controllers
{
    public class MovieController : Controller
    {
		private const int MOVIES_PER_PAGE = 25;

		private readonly IMapper _mapper;
		private readonly IMovieService _movieService;
		private readonly IUserService _userService;

		public MovieController(
			IMovieService movieService,
			IUserService userService,
			IMapper mapper)
		{
			_movieService = movieService;
			_userService = userService;
			_mapper = mapper;
		}

		[HttpGet]
        public async Task<IActionResult> Index(int page)
        {
			PagedList<Movie> movies = await _movieService.GetAllAsync(new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Title"] = "Danh sách phim";
            ViewData["Filter"] = "Phim";

            return View(_mapper.Map<PagedList<MovieViewModel>>(movies));
        }

        [HttpGet]
        [Route("/Movie/{value}")]
        public async Task<IActionResult> SearchByMovieName(string? value, int page)
        {
            ViewData["Filter"] = "Phim có tên";
			ViewData["Title"] = value;

            PagedList<Movie> movies = await _movieService.SearchAsync(value, new PagingParameter(page, MOVIES_PER_PAGE));                    

            ViewData["Action"] = "SearchByMovieName";
            ViewData["RouteValue"] = value;

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet]
		[Route("/Movie/phim-le")]
		public async Task<IActionResult> GetPhimLe(int page)
		{
			PagedList<Movie> movies = await _movieService.FindByTypeAsync("Phim lẻ", new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetPhimLe";
			ViewData["Title"] = "Phim lẻ";
			ViewData["Filter"] = "Phim";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet]
		[Route("/Movie/phim-bo")]
		public async Task<IActionResult> GetPhimBo(int page)
		{
			PagedList<Movie> movies = await _movieService.FindByTypeAsync("Phim bộ", new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetPhimBo";
			ViewData["Title"] = "Phim bộ";
			ViewData["Filter"] = "Phim";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet]
        [Route("/Movie/year/{year}")]
        public async Task<IActionResult> GetMovieByReleaseYear(int year, int page)
        {
            PagedList<Movie> movies = await _movieService.FindByYearAsync(year, new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetMovieByReleaseYear";
			ViewData["Title"] = year.ToString();
            ViewData["Filter"] = "Phim ra mắt năm";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet]
		[Route("/Movie/before-year/{year}")]
		public async Task<IActionResult> GetMovieBeforeYear(int year, int page)
		{
			PagedList<Movie> movies = await _movieService.FindBeforeYearAsync(year, new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetMovieBeforeYear";
			ViewData["Title"] = year.ToString();
			ViewData["Filter"] = "Phim ra mắt trước năm";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[Route("/Movie/tag/{value}")]
		public async Task<IActionResult> GetMovieByTag(string? value, int page)
		{
			PagedList<Movie> movies = await _movieService.FindByTagAsync(value ?? "", new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetMovieByTag";
			ViewData["Title"] = value;
			ViewData["Filter"] = "Phim có Tag";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

        [Route("/Movie/top-rating")]
        public async Task<IActionResult> GetTopRatingMovie(int page)
        {
			PagedList<Movie> movies = await _movieService.GetMoviesOrderByRatingAsync(new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetTopRatingMovie";
			ViewData["Title"] = "Top Rating";
            ViewData["Filter"] = "Phim";

            return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
        }

        [Route("/Movie/trailer")]
        public async Task<IActionResult> GetTrailer(int page)
		{
			PagedList<Movie> movies = await _movieService.GetTrailerAsync(new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetTrailer";
			ViewData["Title"] = "";
            ViewData["Filter"] = "Trailer";

            return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
        }

        [HttpGet]
		public async Task<IActionResult> Detail(string id)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Casts,
				m => m.Categories,
				m => m.Country,
				m => m.Directors,
				m => m.Tags,
			});

			if (movie == null)
			{
				return NotFound("Không tìm thấy phim :((");
			}

			return View(_mapper.Map<MovieViewModel>(movie));
		}

		[HttpGet]
		[Route("/Movie/GetRelateMovies")]
		public async Task<IActionResult> GetRelateMovies(string movieid)
		{
			List<Movie> movies = (await _movieService.GetRelateMoviesAsync(movieid, 10)).ToList();

			return this.PartialView("_RelatedMoviePartial", _mapper.Map<List<MovieViewModel>>(movies));
		}

		[HttpGet]
		[Route("/Movie/GetLikeButton")]
		public async Task<string> GetLikeButton(string movieid)
		{
			Movie? movie = await _movieService.GetByIdAsync(movieid, new Expression<Func<Movie, object?>>[]
			{
				m => m.LikedUsers
			});

			if (movie == null)
			{
				return $"<a id='like-movie-btn' href=\"#\" title=\"thích\" class=\"btn btn-success movie-detail-btn\">\r\n\t\t\t\t<i class=\"bi bi-hand-thumbs-up-fill\"></i>\r\n\t\t\t\t<strong>Thích</strong>\r\n\t\t\t\t<span class=\"ms-1\">0</span>\r\n\t\t\t</a>";
			}

			User? user = await _userService.GetByClaims(User);
			movie.LikedUsers ??= new List<User>();

			if (user == null)
			{
				return $"<a id='like-movie-btn' href=\"#\" title=\"thích\" class=\"btn btn-success movie-detail-btn\">\r\n\t\t\t\t<i class=\"bi bi-hand-thumbs-up-fill\"></i>\r\n\t\t\t\t<strong>Thích</strong>\r\n\t\t\t\t<span class=\"ms-1\">{movie.LikedUsers.Count}</span>\r\n\t\t\t</a>";
			}

			string like = movie.LikedUsers.Contains(user) ? "Đã thích" : "Thích";

			return $"<a id='like-movie-btn' href=\"#\" title=\"thích\" class=\"btn btn-success movie-detail-btn\">\r\n\t\t\t\t<i class=\"bi bi-hand-thumbs-up-fill\"></i>\r\n\t\t\t\t<strong>{like}</strong>\r\n\t\t\t\t<span class=\"ms-1\">{movie.LikedUsers.Count}</span>\r\n\t\t\t</a>";
		}

		[HttpPost]
		[Route("/Movie/LikeMovie")]
		public async Task<JsonResult> LikeMovie(string movieid)
		{
			User? user = await _userService.GetByClaims(User);
			
			if(user == null)
			{
                return Json(new { success = false, notsignin = true });
            }

			bool like = true;
            try
            {
				like = await _movieService.AddLikedUserAsync(movieid, user);
			}
			catch
			{
                return Json(new { success = false });
            }

            return Json(new {success = true, like = like});
		}

		[HttpGet]
		[Route("/Movie/Watch/{id}/{episode?}")]
		public async Task<IActionResult> Watch(string id, int episode)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Videos
			});

			if (movie == null)
			{
				return NotFound();
			}

			Video? video = movie.Videos?.FirstOrDefault(v => v.Episode == episode);

			return View(new WatchMovieViewModel(movie.Id, movie.TranslateName, movie.Type)
			{
				MovieImage = movie.Image,
				VideoUrl = video?.VideoUrl,
				MovieEpisodeCount = movie.EpisodeCount,
				Episode = video?.Episode ?? 0
			});
		}

		[HttpPost]
		[Route("/Movie/IncreaseView")]
		public JsonResult IncreaseView(string id)
		{
			try
			{
				_movieService.IncreateViewAsync(id);
			}
			catch
			{
				return Json("");
			}

			return Json("");
		}
	}
}
