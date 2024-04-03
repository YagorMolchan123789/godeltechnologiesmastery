using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MainDbContext _context;

        public UnitOfWork(MainDbContext context)
        {
            _context = context;
        }

        private IShoesRepository _shoeRepository;

        public IShoesRepository Shoes
        {
            get
            {
                if (_shoeRepository == null)
                {
                    _shoeRepository = new ShoesRepository(_context);
                }

                return _shoeRepository;
            }
        }

        private IUserRepository _userRepository;

        public IUserRepository Users
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                }

                return _userRepository;
            }
        }

        public MainDbContext Context => _context;        

        public void Dispose()
        {
            _context.Dispose();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
