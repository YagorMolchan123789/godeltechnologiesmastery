using AutoMapper;
using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Business.Extensions;
using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Entities;
using GTE.Mastery.ShoeStore.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Shoe> CreateShoeAsync(CreateEditShoeDto shoeDto)
        {
            var shoe = _mapper.Map<Shoe>(shoeDto);
            _unitOfWork.Shoes.Add(shoe);
            _unitOfWork.SaveChanges();
            return shoe;
        }

        public async Task DeleteShoeAsync(int shoeId)
        {
            var shoe = _unitOfWork.Shoes.Get(shoeId);
            _unitOfWork.Shoes.Remove(shoe);
            _unitOfWork.SaveChanges();
        }

        public async Task<ShoeDto> GetShoeAsync(int shoeId)
        {
            var shoe = _unitOfWork.Shoes.Get(shoeId);
            var shoeDto = _mapper.Map<ShoeDto>(shoe);
            return shoeDto;
        }

        public async Task<IEnumerable<ShoeDto>> ListShoesAsync(int? skip = null, int? take = null)
        {
            var shoes = _unitOfWork.Shoes.GetShoes(skip, take);
            var shoeDtos = _mapper.Map<List<ShoeDto>>(shoes);
            return shoeDtos;
        }

        public async Task<Shoe> UpdateShoeAsync(int id, CreateEditShoeDto shoeDto)
        {
            var shoe = _unitOfWork.Shoes.Get(id);

            if (shoe != null)
            {
                shoe.Name = shoeDto.Name;
                shoe.Vendor = shoeDto.Vendor;
                shoe.Price = shoeDto.Price;
                shoe.BrandId = shoeDto.BrandId;
                shoe.SizeId = shoeDto.SizeId;
                shoe.CategoryId = shoeDto.CategoryId;
                shoe.ColorId = shoeDto.ColorId;
                shoe.Gender = shoeDto.Gender;
            }

            _unitOfWork.Shoes.Update(shoe);
            _unitOfWork.SaveChanges();

            return shoe;
        }

        public void InitializeDto(CreateEditShoeDto shoeDto)
        {
            shoeDto.Genders = Gender.Male.ToSelectList();
            shoeDto.Sizes = new SelectList(_unitOfWork.Sizes.GetAll(), "Id", "Value");
            shoeDto.Categories = new SelectList(_unitOfWork.Categories.GetAll(), "Id", "Name");
            shoeDto.Brands = new SelectList(_unitOfWork.Brands.GetAll(), "Id", "Name");
            shoeDto.Colors = new SelectList(_unitOfWork.Colors.GetAll(), "Id", "Name");
        }
    }
}
