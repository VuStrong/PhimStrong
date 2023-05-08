using PhimStrong.Application.Models;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using System.Security.Claims;

namespace PhimStrong.Application.Interfaces
{
    public interface IUserService
	{
        Task<User?> FindByIdAsync(string id);
        Task<User?> GetByClaims(ClaimsPrincipal claims);
		bool IsSignIn(ClaimsPrincipal claims);
		Task<bool> IsInRoleAsync(User? user, string role);
		Task<bool> IsLockedOutAsync(User user);
        Task<PagedList<User>> SearchAsync(PagingParameter pagingParameter, string? value = null, string? role = null);
		Task<IEnumerable<string>> GetRolesAsync(User user);
		Task<Result> ChangePasswordAsync(string userid, string oldPasswd, string newPasswd);
		Task<Result> ChangeEmailAsync(string userid, string email);
        Task ChangeUserRoleAsync(string userid, string? role);
        Task ToggleLockUserAsync(string userid);
		Task<Result> UpdateAsync(string userid, User user);
		Task DeleteAsync(string userid);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
    }
}
