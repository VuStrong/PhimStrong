using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhimStrong.Areas.Admin.Models;
using PhimStrong.Data;
using SharedLibrary.Models;

namespace PhimStrong.Areas.Admin.Components
{
    public class ModalCastViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public ModalCastViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string movieid)
        {
            Movie? movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieid);

            return View(new ModalCastModel
            {
                SelectedCasts = movie?.Casts?.Select(c => c.Name).ToList(),
                Casts = _db.Casts.ToList()
            });
        }
    }
}
