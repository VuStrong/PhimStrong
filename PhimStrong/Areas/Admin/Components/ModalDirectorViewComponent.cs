using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhimStrong.Areas.Admin.Models;
using PhimStrong.Data;
using SharedLibrary.Models;

namespace PhimStrong.Areas.Admin.Components
{
    public class ModalDirectorViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public ModalDirectorViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string movieid)
        {
            Movie? movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieid);

            return View(new ModalDirectorModel
            {
                SelectedDirectors = movie?.Directors?.Select(c => c.Name).ToList(),
                Directors = _db.Directors.ToList()
            });
        }
    }
}
