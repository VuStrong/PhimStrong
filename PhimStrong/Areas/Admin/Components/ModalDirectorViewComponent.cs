using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Areas.Admin.Models;
using PhimStrong.Domain.Models;
using System.Linq.Expressions;

namespace PhimStrong.Areas.Admin.Components
{
    public class ModalDirectorViewComponent : ViewComponent
    {
        private readonly IMovieService _movieService;
        private readonly IDirectorService _directorService;

        public ModalDirectorViewComponent(IMovieService movieService, IDirectorService directorService)
        {
            _movieService = movieService;
            _directorService = directorService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string movieid)
        {
            Movie? movie = await _movieService.GetByIdAsync(movieid, new Expression<Func<Movie, object?>>[]
            {
                m => m.Directors
            });

            return View(new ModalDirectorModel
            {
                SelectedDirectors = movie?.Directors?.Select(d => d.Name).ToList(),
                Directors = (await _directorService.GetAllAsync()).Select(d => d.Name).ToList()
            });
        }
    }
}
