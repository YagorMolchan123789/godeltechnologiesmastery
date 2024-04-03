using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Data.Repositories
{
    public class ShoesRepository : Repository<Shoe>, IShoesRepository
    {
        public ShoesRepository(MainDbContext context) : base(context)
        {

        }

        public bool IsUnique(int id, string name, int sizeId, int colorId)
        {
            Shoe shoe = _context.Set<Shoe>().FirstOrDefault(s => s.Name == name && s.SizeId == sizeId && s.ColorId == colorId);

            if (shoe != null && shoe.Id != id)
            {
                return false;
            }
            
            return true;
        }

        public IEnumerable<Shoe> GetShoes(int? skip = null, int? take = null)
        {
            var query = _context.Set<Shoe>().AsQueryable();

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
    }
}
