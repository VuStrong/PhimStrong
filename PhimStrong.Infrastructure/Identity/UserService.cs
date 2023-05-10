using Microsoft.AspNetCore.Identity;
using PhimStrong.Application.Interfaces;
using PhimStrong.Application.Models;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using SharedLibrary.Helpers;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace PhimStrong.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result> ChangeEmailAsync(string userid, string email)
        {
            User? user = await this.FindByIdAsync(userid);

            if (user == null)
            {
                return Result.Error("Unable to load user.");
            }

            var emailUser = await _userManager.FindByEmailAsync(email);
            if (emailUser != null)
            {
                return Result.Error($"Email {email} đã tồn tại.");
            }

            if (email != user.Email)
            {
                user.Email = email;
                user.EmailConfirmed = false;

                var result = await _userManager.UpdateAsync(user);

                return Result.ToAppResult(result);
            }

            return Result.OK();
        }

		public async Task<Result> ConfirmEmailAsync(string userid, string token)
		{
			var user = await _userManager.FindByIdAsync(userid);

			if (user == null) return Result.Error();

			var result = await _userManager.ConfirmEmailAsync(user, token);

			return Result.ToAppResult(result);
		}

		public async Task<Result> ChangePasswordAsync(string userid, string oldPasswd, string newPasswd)
        {
            User? user = await this.FindByIdAsync(userid);

            if (user == null) return Result.Error("Unable to load user.");

            var result = await _userManager.ChangePasswordAsync(user, oldPasswd, newPasswd);

            return Result.ToAppResult(result);
        }

        public async Task<Result> ChangeUserRoleAsync(string userid, string? role)
        {
            User? user = await this.FindByIdAsync(userid);

            if (user == null) return Result.Error("User not found");

            user.RoleName = null;
            var result = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            if (!result.Succeeded)
            {
                return Result.ToAppResult(result);
            }

            if (role != null)
            {
                result = await _userManager.AddToRoleAsync(user, role);

                if (!result.Succeeded)
                {
					return Result.ToAppResult(result);
				}

				user.RoleName = role;
				result = await _userManager.UpdateAsync(user);
            }

			return Result.ToAppResult(result);
		}

		public async Task<Result> DeleteAsync(string userid)
        {
            User user = await _userManager.FindByIdAsync(userid);

            if (user == null) return Result.Error("User not found");

            IEnumerable<Comment> comments = await _unitOfWork.CommentRepository.GetAsync(
                c => c.User.Id == userid,
                includes: new Expression<Func<Comment, object?>>[]
                {
                    c => c.Responses
                });

            foreach(var comment in comments)
            {
                if (comment.Responses != null)
                {
                    foreach (var resComment in comment.Responses)
                    {
                        _unitOfWork.CommentRepository.Delete(resComment);
                    }
                }
            }

			var result = await _userManager.DeleteAsync(user);

			return Result.ToAppResult(result);
		}

		public async Task<User?> FindByEmailAsync(string email)
		{
			return await _userManager.FindByEmailAsync(email);
		}

		public async Task<User?> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

		public async Task<string> GeneratePasswordResetTokenAsync(User user)
		{
            return await _userManager.GeneratePasswordResetTokenAsync(user);
		}

		public async Task<User?> GetByClaims(ClaimsPrincipal claims)
        {
            return await _userManager.GetUserAsync(claims);
        }

        public async Task<IEnumerable<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(User? user, string role)
        {
            return user != null && await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> IsLockedOutAsync(User user)
        {
            return await _userManager.IsLockedOutAsync(user);
        }

        public bool IsSignIn(ClaimsPrincipal claims)
        {
            return _signInManager.IsSignedIn(claims);
        }

		public async Task<Result> ResetPasswordAsync(string userEmail, string code, string newPassword)
		{
			var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null) return Result.Error();

			var result = await _userManager.ResetPasswordAsync(user, code, newPassword);

            return Result.ToAppResult(result);
        }

		public async Task<PagedList<User>> SearchAsync(PagingParameter pagingParameter, string? value = null, string? role = null)
        {
            IQueryable<User> users = _userManager.Users;

            if (role != null)
            {
                users = users.Where(u => u.RoleName == role);
            }

            if (value != null)
            {
                value = value.RemoveMarks();
                users = users.Where(u => (u.NormalizeDisplayName ?? "").Contains(value));
            }

            return await PagedList<User>.ToPagedList(
                users, pagingParameter.Page, pagingParameter.Size, pagingParameter.AllowCalculateCount);
        }

        public async Task<Result> ToggleLockUserAsync(string userid)
        {
            User? user = await _userManager.FindByIdAsync(userid);

            if (user == null) return Result.Error("User not found");

			var result = await _userManager.SetLockoutEnabledAsync(user, true);

            if (!result.Succeeded) return Result.ToAppResult(result);

			if (await _userManager.IsLockedOutAsync(user))
            {
                result = await _userManager.SetLockoutEndDateAsync(user, new DateTime(2020, 12, 20));
            }
            else
            {
                result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(10));
            }

			return Result.ToAppResult(result);
		}

        public async Task<Result> UpdateAsync(string userid, User user)
        {
            User? userToEdit = await this.FindByIdAsync(userid);

            if (userToEdit == null) return Result.Error("User not found");

            Regex validatePhoneNumberRegex = new Regex("^\\+?\\d{1,4}?[-.\\s]?\\(?\\d{1,3}?\\)?[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,9}$");
            if (user.PhoneNumber != null && user.PhoneNumber != userToEdit.PhoneNumber && validatePhoneNumberRegex.IsMatch(user.PhoneNumber))
            {
                var rel = await _userManager.SetPhoneNumberAsync(userToEdit, user.PhoneNumber);
                if (rel.Succeeded) userToEdit.PhoneNumberConfirmed = true;
            }

            if (user.DisplayName != null && user.DisplayName != userToEdit.DisplayName)
            {
                userToEdit.DisplayName = user.DisplayName;
                userToEdit.NormalizeDisplayName = userToEdit.DisplayName.RemoveMarks();
            }

            if (user.Hobby != null && user.Hobby != userToEdit.Hobby)
            {
                userToEdit.Hobby = user.Hobby;
            }

            if (user.FavoriteMovie != null && user.FavoriteMovie != userToEdit.FavoriteMovie)
            {
                userToEdit.FavoriteMovie = user.FavoriteMovie;
            }

            var result = await _userManager.UpdateAsync(userToEdit);

            return Result.ToAppResult(result);
        }
    }
}
