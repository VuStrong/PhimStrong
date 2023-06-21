using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Models.Movie;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using AutoMapper;
using System.Linq.Expressions;
using System.Security.Claims;
using PhimStrong.Domain.Parameters;
using PhimStrong.Resources.Movie;

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

            return View(_mapper.Map<PagedList<MovieViewModel>>(movies));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Search([FromQuery(Name = "q")] string? value, int page)
        {
			ViewData["Title"] = $"Từ khóa: {value}";

			PagedList<Movie> movies = await _movieService.SearchAsync(new MovieParameter(page, MOVIES_PER_PAGE)
			{
				Value = value,
				OrderBy = "CreatedDate_desc"
			});

			ViewData["Action"] = "SearchByMovieName";
			ViewData["q"] = value;

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet("[action]")]
		public async Task<IActionResult> AdvancedSearch(MovieParameterResource movieParameterResource)
		{
            ViewData["Title"] = "Tìm kiếm nâng cao";

			movieParameterResource.Size = MOVIES_PER_PAGE;

            MovieParameter movieParameter = _mapper.Map<MovieParameter>(movieParameterResource);

            PagedList<Movie> movies = await _movieService.SearchAsync(movieParameter);

			ViewData["Action"] = "AdvancedSearch";

			if (movieParameterResource.Year > 0) 
				ViewData["Year"] = movieParameterResource.Year;
			
			ViewData["Country"] = movieParameterResource.Country;
			ViewData["OrderBy"] = movieParameterResource.OrderBy;
			ViewData["Type"] = movieParameterResource.Type;
			ViewData["Categories"] = movieParameterResource.Categories;

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet("phim-le")]
		public async Task<IActionResult> GetPhimLe(int page)
		{
			PagedList<Movie> movies = await _movieService.FindByTypeAsync("Phim lẻ", new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetPhimLe";
			ViewData["Title"] = "Phim lẻ";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet("phim-bo")]
		public async Task<IActionResult> GetPhimBo(int page)
		{
			PagedList<Movie> movies = await _movieService.FindByTypeAsync("Phim bộ", new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetPhimBo";
			ViewData["Title"] = "Phim bộ";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

        [HttpGet("year-{year}")]
        public async Task<IActionResult> GetMovieByReleaseYear(int year, int page)
        {
            PagedList<Movie> movies = await _movieService.FindByYearAsync(year, new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetMovieByReleaseYear";
            ViewData["Title"] = $"Phim ra mắt năm {year}";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet("before-{year}")]
		public async Task<IActionResult> GetMovieBeforeYear(int year, int page)
		{
			PagedList<Movie> movies = await _movieService.FindBeforeYearAsync(year, new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetMovieBeforeYear";
			ViewData["Title"] = $"Phim ra mắt trước năm {year}";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

		[HttpGet("tag/{tag}")]
		public async Task<IActionResult> GetMovieByTag(string? tag, int page)
		{
			PagedList<Movie> movies = await _movieService.FindByTagAsync(tag ?? "", new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetMovieByTag";
			ViewData["Title"] = $"Phim có Tag: {tag}";

			return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
		}

        [HttpGet("top-rating")]
        public async Task<IActionResult> GetTopRatingMovie(int page)
        {
			PagedList<Movie> movies = await _movieService.GetMoviesOrderByRatingAsync(new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetTopRatingMovie";
			ViewData["Title"] = "Top Rating";

            return View("Index", _mapper.Map<PagedList<MovieViewModel>>(movies));
        }

        [HttpGet("/trailer")]
        public async Task<IActionResult> GetTrailer(int page)
		{
			PagedList<Movie> movies = await _movieService.GetTrailerAsync(new PagingParameter(page, MOVIES_PER_PAGE));

			ViewData["Action"] = "GetTrailer";
            ViewData["Title"] = "Trailer";

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

		[HttpGet("[action]")]
		public async Task<IActionResult> GetRelatedMovies(string movieid)
		{
			List<Movie> movies = (await _movieService.GetRelateMoviesAsync(movieid, 10)).ToList();

			return this.PartialView("_RelatedMoviePartial", _mapper.Map<List<MovieViewModel>>(movies));
		}

		[HttpGet("[action]")]
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

		[HttpPost("[action]")]
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

			return View(new WatchMovieViewModel(movie.Id, movie.TranslateName, movie.Description ?? "", movie.Type)
			{
				MovieImage = movie.Image,
				VideoUrl = video?.VideoUrl,
				MovieEpisodeCount = movie.EpisodeCount,
				Episode = video?.Episode ?? 0
			});
		}

		[HttpPost("[action]")]
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
