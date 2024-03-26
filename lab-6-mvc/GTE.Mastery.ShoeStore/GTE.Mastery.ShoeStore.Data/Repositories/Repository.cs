using GTE.Mastery.ShoeStore.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected MainDbContext _context;

        public Repository(MainDbContext context)
        {
            _context = context;
        }

        public void Add(TEntity entity)
        {
            _context.Add(entity);
        }

        public TEntity Get(int id)
        {
            return _context.Find<TEntity>(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        public void Remove(TEntity entity)
        {
            _context.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Update(entity);
        }
    }
}
