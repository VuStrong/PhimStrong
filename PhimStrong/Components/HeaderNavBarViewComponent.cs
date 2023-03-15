using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhimStrong.Data;
using SharedLibrary.Models;

namespace PhimStrong.Components
{
    public class HeaderNavBarViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager;

        public HeaderNavBarViewComponent(AppDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["Categories"] = await _db.Categories.ToListAsync();
            ViewData["Countries"] = await _db.Countries.ToListAsync();

            ViewData["User"] = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

            List<int> years = new();
            int currentYear = DateTime.Now.Year;
            
            for (int i = currentYear - 22; i <= currentYear; i++)
            {
                years.Add(i);
            }
            ViewData["Years"] = years;
            
            return View();
        }
    }
}
