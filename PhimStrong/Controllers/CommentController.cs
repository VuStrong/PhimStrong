using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Models.Comment;
using PhimStrong.Models;
using SharedLibrary.Constants;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using AutoMapper;
using System.Text.Encodings.Web;

namespace PhimStrong.Controllers
{
	public class CommentController : Controller
	{
#pragma warning disable
		public const int COMMENTS_PER_PAGE = 10;

		private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
		private readonly IMovieService _movieService;
		private readonly ICommentService _commentService;
        private readonly IUserService _userService;

		public CommentController(
            IMapper mapper,
            IMovieService movieService,
			IUserService userService,
			IEmailSender emailSender,
            ICommentService commentService)
		{
			_mapper = mapper;
			_emailSender = emailSender;
			_movieService = movieService;
			_userService = userService;
			_commentService = commentService;
		}

		[HttpGet]
		[Route("/Comment/GetCommentsPartial")]
		public async Task<IActionResult> GetCommentsPartial(int page, string movieid)
		{
			Movie? movie = await _movieService.GetByIdAsync(movieid);

			if (movie == null) return Json("null");

			PagedList<Comment> comments = await _commentService.GetByMovieIdAsync(
				movieid, new PagingParameter(page, COMMENTS_PER_PAGE));

			User? user = await _userService.GetByClaims(User);

			CommentContainerModel model = new()
			{
				IsEnd = page >= comments.TotalPage,
				Comments = _mapper.Map<List<CommentViewModel>>(comments),
				UserLogin = _userService.IsSignIn(User),
				CommentCount = comments.TotalItems,
				RenderCommentOnly = false,
				UserAvatar = user?.Avatar,
				MovieId = movie.Id,
				IsAdmin = user != null && user.RoleName != null && (user.RoleName == RoleConstant.ADMIN || user.RoleName == RoleConstant.THUY_TO)
			};

			Console.WriteLine("page : " + page);
			Console.WriteLine("total : " + comments.TotalPage);
			Console.WriteLine("End ?? : " + model.IsEnd);

			return this.PartialView("_CommentContainerPartial", model);
		}

		[HttpGet]
		[Route("/Comment/LoadMoreComments")]
		public async Task<IActionResult> LoadMoreComments(int page, string movieid)
		{
			Movie? movie = await _movieService.GetByIdAsync(movieid);

			if (movie == null) return Json("null");

			User? user = await _userService.GetByClaims(User);

            PagedList<Comment> comments = await _commentService.GetByMovieIdAsync(
                movieid, new PagingParameter(page, COMMENTS_PER_PAGE));

            CommentContainerModel model = new()
			{
                IsEnd = page >= comments.TotalPage,
                Comments = _mapper.Map<List<CommentViewModel>>(comments),
				RenderCommentOnly = true,
				IsAdmin = user != null && user.RoleName != null && (user.RoleName == RoleConstant.ADMIN || user.RoleName == RoleConstant.THUY_TO)
			};

			return this.PartialView("_CommentContainerPartial", model);
		}

		[HttpPost]
		[Route("/Comment/CreateComment")]
		public async Task<JsonResult> CreateComment(UserCommentModel? model)
		{
			User? user = await _userService.GetByClaims(User);
			Movie? movie = await _movieService.GetByIdAsync(model.MovieId);

			if (user == null || movie == null)
			{
				return Json(new { success = false });
			}

			Comment? responseToComment = model.ResponseToId > 0 ?
				await _commentService.GetByIdAsync(model.ResponseToId) : null;

			Comment comment = new()
			{
				User = user,
				Movie = movie,
				Content = model.Content,
				CreatedAt = DateTime.Now,
				Like = 0,
				ResponseTo = responseToComment
            };

			try
			{
				await _commentService.CreateAsync(comment);
			}
			catch
			{
				return Json(new { success = false });
			}

			//gửi email tới user đc phàn hồi
			if (responseToComment != null)
			{
				var callbackUrl = Url.Action(
					"Detail",
					"Movie",
					values: new { area = "", id = movie.Id },
					protocol: Request.Scheme);

				_emailSender.SendEmailAsync(
					responseToComment.User.Email,
					"Thông báo",
					$"Ai đó vừa phản hồi Comment của bạn về bộ phim {movie.Name}. " +
					$"<a href='{HtmlEncoder.Default.Encode(callbackUrl ?? "")}'>Ấn vào đây</a> để đi đến trang Web PhimStrong."
				);
			}

			return Json(new
			{
				success = true,
				useravatar = user.Avatar ?? "/src/img/UserAvatars/default_avatar.png",
				username = user.DisplayName,
				cmtcontent = model.Content,
				userrole = user.RoleName ?? ""
			});
		}

		[HttpPost]
		[Route("/Comment/LikeComment")]
		public async Task<JsonResult> LikeComment(int commentid)
		{
			try
			{
				await _commentService.LikeComment(commentid);
			}
			catch
			{
				return Json(new { success = false });
			}

			return Json(new {success = true});
		}

        [HttpPost]
        [Route("/Comment/DeleteComment")]
		[Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
		public async Task<JsonResult> DeleteComment(int commentid)
        {
			try
			{
				await _commentService.DeleteAsync(commentid);
			}
			catch
			{
				return Json(new { success = false });
			}

			return Json(new { success = true });
        }
    }
}
