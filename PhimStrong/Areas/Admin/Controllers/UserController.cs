using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhimStrong.Areas.Admin.Models;
using PhimStrong.Data;
using SharedLibrary.Models;
using SharedLibrary.Constants;
using System.Data;
using System.Text.RegularExpressions;
using SharedLibrary.Helpers;

namespace PhimStrong.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
	public class UserController : Controller
	{
		private readonly AppDbContext _db;
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IWebHostEnvironment _environment;

		public UserController(
			AppDbContext db, 
			UserManager<User> userManager, 
			RoleManager<IdentityRole> roleManager, 
			IWebHostEnvironment environment)
		{
			_db = db;
			_userManager = userManager;
			_roleManager = roleManager;
			_environment = environment;
		}

		private const int USERS_PER_PAGE = 15;

		[HttpGet]
		public async Task<IActionResult> Index(int page, string? filter = null)
		{
			if (page <= 0) page = 1;

			int numberOfPages = 0;

            List<User> users = new List<User>();
			int count = 0; // total of search result
			if (filter == null || filter.Trim() == "")
            {
				count = _db.Users.Count();
				numberOfPages = (int)Math.Ceiling((double)count / USERS_PER_PAGE);
				TempData["TotalCount"] = count;

				if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                users = _db.Users.Skip((page - 1) * USERS_PER_PAGE).Take(USERS_PER_PAGE).ToList();
            }
            else
            {
                MatchCollection match = Regex.Matches(filter ?? "", @"^<.+>");

                if (match.Count > 0)
                {
                    string matchValue = new Regex(@"<|>").Replace(match[0].ToString(), "");

					string filterValue = new Regex(@"^<.+>").Replace(filter ?? "", "");
                    switch (matchValue)
                    {
                        case PageFilterConstant.FILTER_BY_ROLE:
                            users = (await _userManager.GetUsersInRoleAsync(filterValue) ?? new List<User>()).ToList();

							TempData["FilterMessage"] = "Role là " + filterValue;

							break;
						case PageFilterConstant.FILTER_BY_NAME:
							TempData["FilterMessage"] = "tên là " + filterValue;
						 	filterValue = filterValue.RemoveMarks();

                            users = _db.Users.ToList().Where(u => 
                                (u.NormalizeDisplayName ?? "").Contains(filterValue)
                            ).ToList();

							break;
                        default:
                            break;
                    }
                }

				count = users.Count;
				TempData["TotalCount"] = count;

				numberOfPages = (int)Math.Ceiling((double)count / USERS_PER_PAGE);
                if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                users = users.Skip((page - 1) * USERS_PER_PAGE).Take(USERS_PER_PAGE).ToList();
            }

            TempData["NumberOfPages"] = numberOfPages;
			TempData["CurrentPage"] = page;
            TempData["filter"] = filter;

            return View(users);
		}

        [HttpGet]
		public async Task<IActionResult> Edit(string? userid)
		{
            if (userid == null)
            {
                return NotFound("Không tìm thấy User :((");
            }

            var myUser = await _userManager.GetUserAsync(User);
            if (myUser == null)
            {
                return NotFound("Không tìm thấy User :((");
            }

            var user = await _userManager.FindByIdAsync(userid);
			if(user == null)
			{
                return NotFound("Không tìm thấy User :((");
            }

			if (await _userManager.IsInRoleAsync(user, RoleConstant.THUY_TO) && !await _userManager.IsInRoleAsync(myUser, RoleConstant.THUY_TO))
			{
                return RedirectToAction("AccessDenied", "Authentication", new
				{ 
					area = "Identity",
					text = $"Oops ! Bạn không được phép chỉnh sửa thông tin của User có Role là {RoleConstant.THUY_TO} :("
				});
            }

            return View(await GetEditUserModel(user));
		}

		[HttpPost]
		public async Task<JsonResult> EditRole(string? userid, EditUserModel model)
		{
			if (model == null || userid == null)
			{
				return Json(new { success = false });
			}

			var user = await _userManager.FindByIdAsync(userid);

			if (user == null)
			{
				return Json(new { success = false });
			}

			user.RoleName = null;
			var result = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            if (!result.Succeeded)
            {
				return Json(new { success = false });
			}

			if (model.UserRole != null && model.UserRole != "none")
			{
				result = await _userManager.AddToRoleAsync(user, model.UserRole);
				
				if (!result.Succeeded)
				{
					return Json(new { success = false });
				}

				user.RoleName = model.UserRole;
				await _userManager.UpdateAsync(user);
			}

			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<JsonResult> ToggleLockUser(string? userid)
		{
			if (userid == null)
			{
				return Json(new { success = false });
			}

			var user = await _userManager.FindByIdAsync(userid);
			
			if (user == null)
			{
				return Json(new { success = false });
			}


		 	var result = await _userManager.SetLockoutEnabledAsync(user, true);
			if (!result.Succeeded)
			{
				return Json(new { success = false });
			}

			if (await _userManager.IsLockedOutAsync(user))
			{
				result = await _userManager.SetLockoutEndDateAsync(user, new DateTime(2020, 12, 20));
			}
			else
			{
				result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(10));
			}

			if (!result.Succeeded)
			{
				return Json(new { success = false });
			}

			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string userid)
		{
			if (userid == null)
			{
				TempData["error"] = "Xóa tài khoản thất bại.";
			}

			var user = await _userManager.FindByIdAsync(userid);

			if (user == null)
			{
				TempData["error"] = "Xóa tài khoản thất bại.";
				return RedirectToAction("Index");
			}

			try
			{
				var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/CastAvatars", user.Id + ".jpg");

				FileInfo fileInfo = new FileInfo(file);
				fileInfo.Delete();
			}
			catch { }

			var result = await _userManager.DeleteAsync(user);

			if (!result.Succeeded)
			{
				TempData["error"] = "Xóa tài khoản thất bại.";
				return RedirectToAction("Index");
			}

			TempData["success"] = "Xóa tài khoản thành công.";
			return RedirectToAction("Index");
		}

		private async Task<EditUserModel> GetEditUserModel(User user)
        {
			List<string> userRoles = (await _userManager.GetRolesAsync(user)).ToList();

			string? userRole = "none";
			if (userRoles.Count > 0)
			{
				userRole = userRoles[0];
			}

			List<string> roles = new List<string>();
			roles.Add("none");

			var isThuyTo = await _userManager.IsInRoleAsync(await _userManager.GetUserAsync(User), RoleConstant.THUY_TO);

            (await _roleManager.Roles.ToListAsync()).ForEach(r =>
			{
				if (r.Name != RoleConstant.THUY_TO || isThuyTo)
				{
					roles.Add(r.Name);
				}
			});

			return new EditUserModel
            {
                User = user,
                UserRole = userRole,
                RoleList = roles
            };
		}
	}
}
