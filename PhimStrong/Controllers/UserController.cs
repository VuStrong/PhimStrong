using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Models;
using SharedLibrary.Models;
using System.Text.Encodings.Web;

namespace PhimStrong.Controllers
{
#pragma warning disable
	public class UserController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly IEmailSender _emailSender;

		public UserController(UserManager<User> userManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_emailSender = emailSender;
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

		[Route("/User/Report")]
		public async Task<JsonResult> Report(ReportModel model)
		{
			_emailSender.SendEmailAsync(
				"vubamanh05@gmail.com",
				"Báo lỗi",
				$"Người dùng có Email {model.Email} báo lỗi : {model.Content}"
			);

			return Json(new
			{
				success = true
			});
		}
	}
}
