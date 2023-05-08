using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Areas.Admin.Models;
using PhimStrong.Domain.Models;
using System.Linq.Expressions;

namespace PhimStrong.Areas.Admin.Components
{
    public class ModalCategoryViewComponent : ViewComponent
    {
        private readonly IMovieService _movieService;
        private readonly ICategoryService _categoryService;

        public ModalCategoryViewComponent(IMovieService movieService, ICategoryService categoryService)
        {
            _movieService = movieService;
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string movieid)
        {
            Movie? movie = await _movieService.GetByIdAsync(movieid, new Expression<Func<Movie, object?>>[]
            {
                m => m.Categories
            });

            return View(new ModalCategoryModel
            {
                SelectedCategories = movie?.Categories?.Select(c => c.Name).ToList(),
                Categories = (await _categoryService.GetAllAsync()).Select(c => c.Name).ToList()
            });
        }
    }
}
