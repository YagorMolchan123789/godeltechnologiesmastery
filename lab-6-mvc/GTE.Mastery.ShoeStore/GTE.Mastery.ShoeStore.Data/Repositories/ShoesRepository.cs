using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GTE.Mastery.ShoeStore.Data.Repositories
{
    public class ShoesRepository : IShoesRepository
    {
        private readonly MainDbContext _dbContext;

        public ShoesRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public bool IsUnique(int id, string name, int sizeId, int colorId)
        {
            Shoe shoe = _dbContext.Set<Shoe>().FirstOrDefault(s => s.Name == name && s.SizeId == sizeId && s.ColorId == colorId);

            if (shoe != null && shoe.Id != id)
            {
                return false;
            }
            
            return true;
        }

        public IEnumerable<Shoe> Get(int? skip = null, int? take = null)
        {
            var query = _dbContext.Set<Shoe>().AsQueryable();

            if (skip != null && skip > 0)
            {
                query = query?.Skip(skip.Value);
            }

            if (take != null && take > 0)
            {
                query = query?.Take(take.Value);
            }

            return query?.ToList();
        }

        public Shoe Get(int id)
        {
            return _dbContext.Set<Shoe>().Find(id);
        }

        public void Add(Shoe shoe)
        {
            _dbContext.Add(shoe);
        }

        public void Update(Shoe shoe)
        {
            _dbContext.Update(shoe);
        }

        public void Delete(int id)
        {
            var shoe = _dbContext.Set<Shoe>().Attach(new Shoe { Id = id });
            shoe.State = EntityState.Deleted;
        }
    }
}
