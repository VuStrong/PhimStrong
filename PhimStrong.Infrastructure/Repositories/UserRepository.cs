using PhimStrong.Domain.Models;
using PhimStrong.Infrastructure.Context;

namespace PhimStrong.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(PhimStrongDbContext context) : base(context) {}
    }
}
