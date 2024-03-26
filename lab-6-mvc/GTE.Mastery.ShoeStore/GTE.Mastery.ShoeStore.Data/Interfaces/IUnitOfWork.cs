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

        IRepository<Size> Sizes { get; }

        IRepository<Brand> Brands { get; }

        IRepository<Category> Categories { get; }

        IRepository<Color> Colors { get; }

        void SaveChanges();
    }
}
