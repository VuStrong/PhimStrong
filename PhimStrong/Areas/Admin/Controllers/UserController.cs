using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Areas.Admin.Models;
using SharedLibrary.Constants;
using PhimStrong.Domain.Models;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.PagingModel;
using AutoMapper;
using PhimStrong.Models.User;
using PhimStrong.Domain.Parameters;

namespace PhimStrong.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
	public class UserController : Controller
	{
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IWebHostEnvironment _environment;

		public UserController(
            IMapper mapper,
            IUserService userService,
            IRoleService roleService,
            IWebHostEnvironment environment)
		{
			_mapper = mapper;
			_userService = userService;
			_roleService = roleService;
			_environment = environment;
		}

		private const int USERS_PER_PAGE = 15;

		[HttpGet]
		public async Task<IActionResult> Index(int page, string? role = null, string? value = null)
		{
			PagedList<User> users = await _userService.SearchAsync(new PagingParameter(page, USERS_PER_PAGE), value, role);
            
			if (role != null) ViewData["role"] = role;
			if (value != null) ViewData["value"] = value;

			ViewData["roles"] = (await _roleService.GetRolesAsync()).ToList();

            return View(_mapper.Map<PagedList<UserViewModel>>(users));
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string? userid)
		{
			var myUser = await _userService.GetByClaims(User);
			var user = await _userService.FindByIdAsync(userid ?? "");

			if (user == null || myUser == null)
			{
				return NotFound("Không tìm thấy User :((");
			}

			if (await _userService.IsInRoleAsync(user, RoleConstant.THUY_TO) && !await _userService.IsInRoleAsync(myUser, RoleConstant.THUY_TO))
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
			string? role = model.UserRole != null && model.UserRole != "none" ? model.UserRole : null;

			var result = await _userService.ChangeUserRoleAsync(userid ?? "", role);
			
			if (!result.Success) return Json(new { success = false });

			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<JsonResult> ToggleLockUser(string? userid)
		{
			var result = await _userService.ToggleLockUserAsync(userid ?? "");
			
			if (!result.Success) return Json(new { success = false });

			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string userid)
		{
			var result = await _userService.DeleteAsync(userid);

			if (!result.Success)
			{
				TempData["error"] = "Xóa tài khoản thất bại.";
				return RedirectToAction("Index", new { area = "Admin" });
			}

			var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/UserAvatars", userid + ".jpg");

			FileInfo fileInfo = new(file);
			fileInfo.Delete();

			TempData["success"] = "Xóa tài khoản thành công.";
            return RedirectToAction("Index", new { area = "Admin" });
        }

        private async Task<EditUserModel> GetEditUserModel(User user)
		{
			List<string> userRoles = (await _userService.GetRolesAsync(user)).ToList();

			string? userRole = "none";
			if (userRoles.Count > 0)
			{
				userRole = userRoles[0];
			}

			List<string> roles = new()
			{
				"none"
			};

			var isThuyTo = await _userService.IsInRoleAsync(await _userService.GetByClaims(User), RoleConstant.THUY_TO);

			(await _roleService.GetRolesAsync()).ToList().ForEach(r =>
			{
				 if (r != RoleConstant.THUY_TO || isThuyTo)
				 {
					 roles.Add(r);
				 }
			});

			return new EditUserModel
			{
				User = _mapper.Map<UserViewModel>(user),
				UserRole = userRole,
				RoleList = roles,
				IsLock = await _userService.IsLockedOutAsync(user)
			};
		}
	}
}
