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
        Task<IEnumerable<ShoeDto>> ListAsync(int? skip = null, int? take = null);

        Task<ShoeDto> GetAsync(int shoeId);

        Task<Shoe> CreateAsync(UpdateShoeDto shoeDto);

        Task<Shoe> UpdateAsync(int id, UpdateShoeDto shoeDto);

        Task DeleteAsync(int shoeId);
    }
}
