using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Areas.Admin.Models;
using PhimStrong.Domain.Models;
using System.Linq.Expressions;

namespace PhimStrong.Areas.Admin.Components
{
    public class ModalCastViewComponent : ViewComponent
    {
        private readonly IMovieService _movieService;
        private readonly ICastService _castService;

        public ModalCastViewComponent(IMovieService movieService, ICastService castService)
        {
            _movieService = movieService;
            _castService = castService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string movieid)
        {
            Movie? movie = await _movieService.GetByIdAsync(movieid, new Expression<Func<Movie, object?>>[]
            {
                m => m.Casts
            });

            return View(new ModalCastModel
            {
                SelectedCasts = movie?.Casts?.Select(c => c.Name).ToList(),
                Casts = (await _castService.GetAllAsync()).Select(c => c.Name).ToList()
            });
        }
    }
}
