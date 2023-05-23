using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Models.Movie;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using AutoMapper;
using System.Linq.Expressions;
using System.Security.Claims;
using PhimStrong.Domain.Parameters;

namespace PhimStrong.Controllers
{
	[Route("[controller]")]
	public class MovieController : Controller
    {
		private const int MOVIES_PER_PAGE = 25;

		private readonly IMapper _mapper;
		private readonly IMovieService _movieService;

		public MovieController(
			IMovieService movieService,
			IMapper mapper)
		{
			_movieService = movieService;
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

        [HttpGet("{value}")]
        public async Task<IActionResult> SearchByMovieName(string? value, int page)
        {
			ViewData["Filter"] = "Phim có tên";
			ViewData["Title"] = value;

			PagedList<Movie> movies = await _movieService.SearchAsync(new MovieParameter(page, MOVIES_PER_PAGE)
			{
				Value = value,
				OrderBy = "CreatedDate_desc"
			});

			ViewData["Action"] = "SearchByMovieName";
			ViewData["RouteValue"] = value;

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet("phim-le")]
		public async Task<IActionResult> GetPhimLe(int page)
		{
			PagedList<Movie> movies = await _movieService.FindByTypeAsync("Phim lẻ", new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetPhimLe";
			ViewData["Title"] = "Phim lẻ";
			ViewData["Filter"] = "Phim";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet("phim-bo")]
		public async Task<IActionResult> GetPhimBo(int page)
		{
			PagedList<Movie> movies = await _movieService.FindByTypeAsync("Phim bộ", new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetPhimBo";
			ViewData["Title"] = "Phim bộ";
			ViewData["Filter"] = "Phim";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

        [HttpGet("year/{year}")]
        public async Task<IActionResult> GetMovieByReleaseYear(int year, int page)
        {
            PagedList<Movie> movies = await _movieService.FindByYearAsync(year, new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetMovieByReleaseYear";
			ViewData["Title"] = year.ToString();
            ViewData["Filter"] = "Phim ra mắt năm";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet("before-year/{year}")]
		public async Task<IActionResult> GetMovieBeforeYear(int year, int page)
		{
			PagedList<Movie> movies = await _movieService.FindBeforeYearAsync(year, new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetMovieBeforeYear";
			ViewData["Title"] = year.ToString();
			ViewData["Filter"] = "Phim ra mắt trước năm";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet("tag/{value}")]
		public async Task<IActionResult> GetMovieByTag(string? value, int page)
		{
			PagedList<Movie> movies = await _movieService.FindByTagAsync(value ?? "", new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetMovieByTag";
			ViewData["Title"] = value;
			ViewData["Filter"] = "Phim có Tag";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

        [HttpGet("top-rating")]
        public async Task<IActionResult> GetTopRatingMovie(int page)
        {
			PagedList<Movie> movies = await _movieService.GetMoviesOrderByRatingAsync(new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetTopRatingMovie";
			ViewData["Title"] = "Top Rating";
            ViewData["Filter"] = "Phim";

            return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
        }

        [HttpGet("/trailer")]
        public async Task<IActionResult> GetTrailer(int page)
		{
			PagedList<Movie> movies = await _movieService.GetTrailerAsync(new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetTrailer";
			ViewData["Title"] = "";
            ViewData["Filter"] = "Trailer";

            return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
        }

		[HttpGet("detail/{id}")]
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
                return View("/Views/Shared/404.cshtml");
            }

            return View(_mapper.Map<MovieViewModel>(movie));
		}

		[HttpGet("get-related-movies")]
		public async Task<IActionResult> GetRelatedMovies(string movieid)
		{
			List<Movie> movies = (await _movieService.GetRelateMoviesAsync(movieid, 10)).ToList();

			return this.PartialView("_RelatedMoviePartial", _mapper.Map<List<MovieViewModel>>(movies));
		}

		[HttpGet("get-like-button")]
		public async Task<IActionResult> GetLikeButton(string movieid)
		{
			Movie? movie = await _movieService.GetByIdAsync(movieid, new Expression<Func<Movie, object?>>[]
			{
				m => m.LikedUsers
			});

			string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

			return this.PartialView("_MovieLikeButton", new MovieLikeButtonViewModel
			{
				MovieExist = movie != null,
				UserLike = movie?.LikedUsers?.Any(u => u.Id == userid) ?? false,
				UserLogin = !String.IsNullOrEmpty(userid),
				LikeCount = movie?.LikedUsers?.Count ?? 0
			});
		}

		[HttpPost("like-movie")]
		public async Task<JsonResult> LikeMovie(string movieid)
		{
			string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
			
			bool like = true;
            try
            {
				like = await _movieService.LikeMovieAsync(movieid, userid);
			}
			catch
			{
                return Json(new { success = false });
            }

            return Json(new {success = true, like = like});
		}

		[HttpGet("watch/{id}/{episode?}")]
		public async Task<IActionResult> Watch(string id, int episode)
		{
			Movie? movie = await _movieService.GetByIdAsync(id, new Expression<Func<Movie, object?>>[]
			{
				m => m.Videos
			});

			if (movie == null)
			{
                return View("/Views/Shared/404.cshtml");
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

		[HttpPost("increase-view")]
		public async Task<JsonResult> IncreaseView(string id)
		{
			try
			{
				await _movieService.IncreateViewAsync(id);
			}
			catch
			{
				return Json("");
			}

			return Json("");
		}
	}
}
