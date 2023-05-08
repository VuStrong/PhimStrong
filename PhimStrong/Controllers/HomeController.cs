using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Models;
using System.Diagnostics;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Models;
using AutoMapper;
using PhimStrong.Models.Movie;

namespace PhimStrong.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;


        public HomeController(IMovieService movieService, IMapper mapper)
        {
            _movieService = movieService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<Movie> randomMovies = (await _movieService.GetRandomMoviesAsync(10)).ToList();

            PagingParameter pagingParameter = new(1, 12);

            Movie[] newMovies = (await _movieService.GetAllAsync(pagingParameter)).ToArray();
            Movie[] topRatingMovies = (await _movieService.GetMoviesOrderByRatingAsync(pagingParameter)).ToArray();
            Movie[] phimLe = (await _movieService.FindByTypeAsync("Phim lẻ", pagingParameter)).ToArray();
            Movie[] phimBo = (await _movieService.FindByTypeAsync("Phim bộ", pagingParameter)).ToArray();

            return View(new HomeViewModel()
            {
                ListRandomMovies = _mapper.Map<List<MovieViewModel>>(randomMovies),
                ListMovieTopRating = _mapper.Map<MovieViewModel[]>(topRatingMovies),
                ListMovieNew = _mapper.Map<MovieViewModel[]>(newMovies),
                ListPhimLe = _mapper.Map<MovieViewModel[]>(phimLe),
                ListPhimBo = _mapper.Map<MovieViewModel[]>(phimBo)
            });
        }

        [Route("/chinh-sach-rieng-tu")]
        public IActionResult Privacy() => View();

        [Route("/dieu-khoan-su-dung")]
        public IActionResult TermsOfUse() => View();

        [Route("/khieu-nai-ban-quyen")]
        public IActionResult License() => View();

        [Route("/contact")]
        public IActionResult Contact() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}