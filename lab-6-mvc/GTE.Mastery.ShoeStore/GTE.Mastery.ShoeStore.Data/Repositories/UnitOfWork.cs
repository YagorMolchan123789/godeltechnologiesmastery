using GTE.Mastery.ShoeStore.Data.Interfaces;

namespace GTE.Mastery.ShoeStore.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MainDbContext _dbContext;

        public UnitOfWork(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IShoesRepository _shoeRepository;

        public IShoesRepository Shoes
        {
            get
            {
                if (_shoeRepository == null)
                {
                    _shoeRepository = new ShoesRepository(_dbContext);
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
                    _userRepository = new UserRepository(_dbContext);
                }

                return _userRepository;
            }
        }

        public MainDbContext Context => _dbContext;        

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
