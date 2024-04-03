using GTE.Mastery.ShoeStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Data.Interfaces
{
    public interface IShoesRepository : IRepository<Shoe>
    {
        IEnumerable<Shoe> GetShoes(int? skip = null, int? take = null);

        bool IsUnique(int id, string name, int sizeId, int colorId);
    }
}
