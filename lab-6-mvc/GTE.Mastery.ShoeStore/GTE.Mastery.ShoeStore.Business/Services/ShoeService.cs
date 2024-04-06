using AutoMapper;
using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Entities;

namespace GTE.Mastery.ShoeStore.Business.Services
{
    public class ShoeService : IShoeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShoeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Shoe> CreateAsync(UpdateShoeDto newShoe)
        {
            var shoe = new Shoe(newShoe.Name, newShoe.Vendor, newShoe.Price, newShoe.ImagePath, newShoe.Gender, 
                newShoe.BrandId, newShoe.CategoryId, newShoe.ColorId, newShoe.SizeId);
            _unitOfWork.Shoes.Add(shoe);
            _unitOfWork.SaveChanges();
            return shoe;
        }

        public async Task DeleteAsync(int shoeId)
        {
            _unitOfWork.Shoes.Delete(shoeId);
            _unitOfWork.SaveChanges();
        }

        public async Task<ShoeDto> GetAsync(int shoeId)
        {
            var shoe = _unitOfWork.Shoes.Get(shoeId);
            var shoeDto = _mapper.Map<ShoeDto>(shoe);
            return shoeDto;
        }

        public async Task<IEnumerable<ShoeDto>> ListAsync(int? skip = null, int? take = null)
        {
            var shoes = _unitOfWork.Shoes.Get(skip, take);
            var shoeDtos = _mapper.Map<List<ShoeDto>>(shoes);
            return shoeDtos;
        }

        public async Task<Shoe> UpdateAsync(int id, UpdateShoeDto updatedShoe)
        {
            var shoe = _unitOfWork.Shoes.Get(id);

            if (shoe != null)
            {
                shoe.Update(updatedShoe.Name, updatedShoe.Vendor, updatedShoe.Price, updatedShoe.Gender, updatedShoe.BrandId, 
                    updatedShoe.CategoryId, updatedShoe.ColorId, updatedShoe.SizeId);
            }

            _unitOfWork.Shoes.Update(shoe);
            _unitOfWork.SaveChanges();

            return shoe;
        }
    }
}
