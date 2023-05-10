using Microsoft.AspNetCore.Authentication;
using PhimStrong.Application.Models;

namespace PhimStrong.Application.Interfaces
{
    public interface IAuthenticationService
    {
		Task<RegisterResult> RegisterAsync(string name, string email, string password);
        Task<LoginResult> LoginAsync(string email, string password, bool rememberMe);
        Task<LoginResult> ExternalLoginAsync();
		Task LogoutAsync();

		Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
		AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
	}
}
