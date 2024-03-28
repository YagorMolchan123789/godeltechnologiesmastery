using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Interfaces
{
    public interface IShoeService
    {
        Task<IEnumerable<ShoeDto>> ListShoesAsync(int? skip = null, int? take = null);

        Task<ShoeDto> GetShoeAsync(int shoeId);

        Task<Shoe> CreateShoeAsync(UpdateShoeDto shoeDto);

        Task<Shoe> UpdateShoeAsync(int id, UpdateShoeDto shoeDto);

        Task DeleteShoeAsync(int shoeId);
    }
}
