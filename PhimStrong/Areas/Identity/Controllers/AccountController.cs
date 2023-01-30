using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NuGet.Common;
using PhimStrong.Areas.Identity.Models;
using SharedLibrary.Helpers;
using SharedLibrary.Models;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
#pragma warning disable

namespace PhimStrong.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
		private readonly IWebHostEnvironment _environment;
		private readonly IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, IWebHostEnvironment environment, IEmailSender emailSender)
        {
            _userManager = userManager;
            _environment = environment;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Route("/Account")]
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
        [Route("/Account/ChangePassword")]
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

        [HttpGet]
        [Route("/Account/Email")]
        public async Task<IActionResult> Email()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            return View(new ManageEmailModel
            {
                IsEmailConfirmed = user.EmailConfirmed,
                Email = user.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ManageEmailModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            var emailUser = await _userManager.FindByEmailAsync(model.NewEmail);
            if (emailUser != null)
            {
                TempData["status"] = $"Lỗi, Email {model.NewEmail} này đã tồn tại.";
                return RedirectToAction("Email");
            }

            if (model.NewEmail != user.Email)
            {
                user.Email = model.NewEmail;
                user.EmailConfirmed = false;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["success"] = "Đã thay đổi Email";
                    TempData["status"] = "Đã thay đổi Email, hãy kiểm tra hòm thư Email để xác thực.";
                }
            }

            return RedirectToAction("Email");
        }

        [HttpPost]
        public async Task<JsonResult> SendEmailVertify()
        {
			var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new {success = false});
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            string callbackUrl = Url.Action("ConfirmEmail", "Authentication",
                    new
                    {
                        area = "Identity",
                        token = token,
                        userid = user.Id
                    },
                    protocol: Request.Scheme
                );

            await _emailSender.SendEmailAsync(
                user.Email,
                "Xác thực Email",
                $"Yô !, click vào <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>đây</a> để xác thực Email của bạn nhé :)"
            );


            return Json(new { success = true });
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
                if (!userModel.AvatarFile.FileName.EndsWith(".png") &&
                    !userModel.AvatarFile.FileName.EndsWith(".jpg"))
                {
                    return Json(new { success = false, error = "Định dạng ảnh phải là .png, .jpg !" });
                }

                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/UserAvatars", user.Id + ".jpg");
                
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await userModel.AvatarFile.CopyToAsync(fileStream);
                }

                if(user.Avatar != "/src/img/UserAvatars/" + user.Id + ".jpg") 
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
                user.NormalizeDisplayName = user.DisplayName.RemoveMarks();
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
                avatar = user.Avatar ?? ""
            });
        }
	}
}
