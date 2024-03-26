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

        private IRepository<Size> _sizeRepository;

        public IRepository<Size> Sizes
        {
            get
            {
                if (_sizeRepository == null)
                {
                    _sizeRepository = new Repository<Size>(_context);
                }

                return _sizeRepository;
            }
        }

        private IRepository<Brand> _brandRepository;

        public IRepository<Brand> Brands
        {
            get
            {
                if (_brandRepository == null)
                {
                    _brandRepository = new Repository<Brand>(_context);
                }

                return _brandRepository;
            }
        }

        private IRepository<Category> _categoryRepository;

        public IRepository<Category> Categories
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new Repository<Category>(_context);
                }

                return _categoryRepository;
            }
        }

        private IRepository<Color> _colorRepository;

        public IRepository<Color> Colors
        {
            get
            {
                if (_colorRepository == null)
                {
                    _colorRepository = new Repository<Color>(_context);
                }

                return _colorRepository;
            }
        }

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
