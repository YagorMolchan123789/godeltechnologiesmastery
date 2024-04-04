using GTE.Mastery.ShoeStore.Domain.Entities;

namespace GTE.Mastery.ShoeStore.Data.Interfaces
{
    public interface IShoesRepository
    {
        IEnumerable<Shoe> Get(int? skip = null, int? take = null);

        Shoe Get(int id);

        void Add(Shoe shoe);

        void Update(Shoe shoe);

        void Delete(int id);

        bool IsUnique(int id, string name, int sizeId, int colorId);
    }
}
