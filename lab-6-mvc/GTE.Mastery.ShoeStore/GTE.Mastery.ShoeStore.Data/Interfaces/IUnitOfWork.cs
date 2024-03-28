using GTE.Mastery.ShoeStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IShoesRepository Shoes { get; }
        MainDbContext Context { get; }
        void SaveChanges();
    }
}
