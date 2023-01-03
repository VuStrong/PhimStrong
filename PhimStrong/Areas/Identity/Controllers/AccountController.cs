using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Areas.Identity.Models;
using SharedLibrary.Models;
using System.Text.RegularExpressions;

namespace PhimStrong.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
		private readonly IWebHostEnvironment _environment;

        public AccountController(UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("Không tìm thấy user :((");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user.");
			}

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["success"] = "Thay đổi mật khẩu thành công !";
                return RedirectToAction("Index");
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            TempData["error"] = "Thay đổi mật khẩu thất bại !";

			return View();
		}

        [HttpPost]
        public async Task<JsonResult> EditInformation(User? userModel)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (userModel == null || user == null)
            {
                return Json(new { success = false });
            }

            if (userModel.AvatarFile != null)
            {
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/UserAvatars", user.Id + ".jpg");
                
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await userModel.AvatarFile.CopyToAsync(fileStream);
                }

                user.Avatar = "/src/img/UserAvatars/" + user.Id + ".jpg";
            }

            Regex validatePhoneNumberRegex = new Regex("^\\+?\\d{1,4}?[-.\\s]?\\(?\\d{1,3}?\\)?[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,9}$");
            if (userModel.PhoneNumber != null && userModel.PhoneNumber != user.PhoneNumber && validatePhoneNumberRegex.IsMatch(userModel.PhoneNumber))
            {
                var rel = await _userManager.SetPhoneNumberAsync(user, userModel.PhoneNumber);
                if(rel.Succeeded) user.PhoneNumberConfirmed = true;
            }

            if (userModel.DisplayName != null && userModel.DisplayName != user.DisplayName)
            {
                user.DisplayName = userModel.DisplayName;
            }

            if (userModel.Hobby != null && userModel.Hobby != user.Hobby)
            {
                user.Hobby = userModel.Hobby;
            }

            if (userModel.FavoriteMovie != null && userModel.FavoriteMovie != user.FavoriteMovie)
            {
                user.FavoriteMovie = userModel.FavoriteMovie;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                displayname = user.DisplayName,
                phone = user.PhoneNumber,
                favoritemovie = user.FavoriteMovie,
                hobby = user.Hobby,
                avatar = user.Avatar
            });
        }
	}
}
