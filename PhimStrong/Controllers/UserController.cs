using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Models;
using PhimStrong.Models.User;

namespace PhimStrong.Controllers
{
#pragma warning disable
	public class UserController : Controller
	{
		private readonly IUserService _userService;
		private readonly IEmailSender _emailSender;
		private readonly IMapper _mapper;

		public UserController(IUserService userService, IEmailSender emailSender, IMapper mapper)
		{
			_userService = userService;
			_emailSender = emailSender;
			_mapper = mapper;
		}

		[Route("/User/{id?}")]
		public async Task<IActionResult> Index(string id)
		{
			User? user = await _userService.FindByIdAsync(id);

			if (user == null)
			{
				return NotFound("Không tìm thấy User :((");
			}

			return View(_mapper.Map<UserViewModel>(user));
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
