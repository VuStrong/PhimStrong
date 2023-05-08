using PhimStrong.Application.Interfaces;
using PhimStrong.Application.Models;

namespace PhimStrong.Infrastructure.Identity
{
    public class AuthenticationService : IAuthenticationService
    {
        public Task<Result> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RegisterAsync(string name, string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
