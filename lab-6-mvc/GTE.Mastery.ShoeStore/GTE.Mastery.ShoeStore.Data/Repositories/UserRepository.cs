using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Entities;

namespace GTE.Mastery.ShoeStore.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MainDbContext _dbContext;

        public UserRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool UserExists(string email)
        {
            return _dbContext.Set<User>().Any(u => u.Email == email);
        }
    }
}
