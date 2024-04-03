using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MainDbContext context) : base(context)
        {
        }

        public bool UserExists(string email)
        {
            return _context.Set<User>().Any(u => u.Email == email);
        }
    }
}
