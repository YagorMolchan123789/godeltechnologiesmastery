using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Services
{
    public class RepositoryFactory<TEntity> : IRepositoryFactory<TEntity> where TEntity : class
    {
        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            Repository = new Repository<TEntity>(serviceProvider.GetRequiredService<IUnitOfWork>().Context);
        }

        public IRepository<TEntity> Repository { get; private set; }

        public IEnumerable<TEntity> GetEntities()
        {
            return Repository.GetAll();
        }
    }
}
