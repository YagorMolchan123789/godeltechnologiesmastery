using Mastery.KeeFi.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected MainDbContext _context;

        public Repository(MainDbContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }

        public T Get(int id)
        {
            return _context.Find<T>(id);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }
    }
}
