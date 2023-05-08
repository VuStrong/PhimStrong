using PhimStrong.Application.Models;

namespace PhimStrong.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result> RegisterAsync(string name, string email, string password);
        Task<Result> LoginAsync(string email, string password);
    }
}
