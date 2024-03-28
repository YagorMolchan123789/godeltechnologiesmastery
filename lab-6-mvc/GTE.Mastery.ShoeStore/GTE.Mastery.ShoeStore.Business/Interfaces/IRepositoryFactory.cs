using GTE.Mastery.ShoeStore.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Interfaces
{
    public interface IRepositoryFactory<TEntity> where TEntity : class
    {
        IRepository<TEntity> Repository { get; }

        IEnumerable<TEntity> GetEntities();
    }
}
