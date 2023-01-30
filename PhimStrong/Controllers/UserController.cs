using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Data;
using SharedLibrary.Models;

namespace PhimStrong.Controllers
{
	public class UserController : Controller
	{
		private readonly AppDbContext _db;
		private readonly UserManager<User> _userManager;

		public UserController(AppDbContext db, UserManager<User> userManager)
		{
			_db = db;
			_userManager = userManager;
		}

		[Route("/User/{id?}")]
		public async Task<IActionResult> Index(string id)
		{
			User user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				return NotFound("Không tìm thấy User :((");
			}

			return View(user);
		}
	}
}
