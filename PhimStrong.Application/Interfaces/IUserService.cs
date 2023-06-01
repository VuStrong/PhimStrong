using PhimStrong.Application.Models;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using System.Security.Claims;

namespace PhimStrong.Application.Interfaces
{
    public interface IUserService
	{
        Task<User?> FindByIdAsync(string id);
        Task<User?> FindByEmailAsync(string email);
		Task<User?> GetByClaims(ClaimsPrincipal claims);
		Task<User?> GetUserWithLikedMovies(string id);
        bool IsSignIn(ClaimsPrincipal claims);
		Task<bool> IsInRoleAsync(User? user, string role);
		Task<bool> IsLockedOutAsync(User user);
        Task<PagedList<User>> SearchAsync(PagingParameter pagingParameter, string? value = null, string? role = null);
		Task<IEnumerable<string>> GetRolesAsync(User user);
		Task<Result> ChangePasswordAsync(string userid, string oldPasswd, string newPasswd);
		Task<Result> ChangeEmailAsync(string userid, string email);
        Task<Result> ChangeUserRoleAsync(string userid, string? role);
        Task<Result> ToggleLockUserAsync(string userid);
		Task<Result> UpdateAsync(string userid, User user);
		Task<Result> DeleteAsync(string userid);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<string> GeneratePasswordResetTokenAsync(User user);
		Task<Result> ResetPasswordAsync(string userEmail, string code, string newPassword);
		Task<Result> ConfirmEmailAsync(string userid, string token);
	}
}
