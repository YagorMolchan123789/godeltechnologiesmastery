using GTE.Mastery.ShoeStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        bool UserExists(string email);
    }
}
