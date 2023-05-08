using Microsoft.AspNetCore.Identity;
using PhimStrong.Application.Interfaces;
using PhimStrong.Application.Models;
using PhimStrong.Domain.Exceptions.NotFound;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using SharedLibrary.Helpers;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace PhimStrong.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result> ChangeEmailAsync(string userid, string email)
        {
            User? user = await this.FindByIdAsync(userid);

            if (user == null)
            {
                return Result.Error(new List<string> { "Unable to load user." });
            }

            var emailUser = await _userManager.FindByEmailAsync(email);
            if (emailUser != null)
            {
                return Result.Error(new List<string> { $"Email {email} đã tồn tại." });
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

        public async Task<Result> ChangePasswordAsync(string userid, string oldPasswd, string newPasswd)
        {
            User? user = await this.FindByIdAsync(userid);

            if (user == null)
            {
                return Result.Error(new List<string> { "Unable to load user." });
            }

            var result = await _userManager.ChangePasswordAsync(user, oldPasswd, newPasswd);

            return Result.ToAppResult(result);
        }

        public async Task ChangeUserRoleAsync(string userid, string? role)
        {
            User? user = await this.FindByIdAsync(userid);

            if (user == null) throw new UserNotFoundException(userid);

            user.RoleName = null;
            var result = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            if (!result.Succeeded)
            {
                throw new Exception();
            }

            if (role != null)
            {
                result = await _userManager.AddToRoleAsync(user, role);

                if (!result.Succeeded)
                {
                    throw new Exception();
                }

                user.RoleName = role;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task DeleteAsync(string userid)
        {
            User user = await _userManager.FindByIdAsync(userid);

            if (user == null) throw new UserNotFoundException(userid);

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception();
            }
        }

        public async Task<User?> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
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

            return await PagedList<User>.ToPagedList(users, pagingParameter.Page, pagingParameter.Size);
        }

        public async Task ToggleLockUserAsync(string userid)
        {
            User? user = await _userManager.FindByIdAsync(userid);

            if (user == null) throw new UserNotFoundException(userid);

            var result = await _userManager.SetLockoutEnabledAsync(user, true);

            if (!result.Succeeded) throw new Exception();

            if (await _userManager.IsLockedOutAsync(user))
            {
                result = await _userManager.SetLockoutEndDateAsync(user, new DateTime(2020, 12, 20));
            }
            else
            {
                result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(10));
            }

            if (!result.Succeeded) throw new Exception();
        }

        public async Task<Result> UpdateAsync(string userid, User user)
        {
            User? userToEdit = await this.FindByIdAsync(userid);

            if (userToEdit == null)
            {
                return Result.Error(new List<string> { "User not found" });
            }

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
