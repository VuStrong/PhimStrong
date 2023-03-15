using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhimStrong.Areas.Admin.Models;
using PhimStrong.Data;
using SharedLibrary.Models;

namespace PhimStrong.Areas.Admin.Components
{
    public class ModalCategoryViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public ModalCategoryViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string movieid)
        {
            Movie? movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieid);

            return View(new ModalCategoryModel
            {
                SelectedCategories = movie?.Categories?.Select(c => c.Name).ToList(),
                Categories = _db.Categories.ToList()
            });
        }
    }
}
