using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Services
{
    public class DataHelper : IDataHelper
    {
        private readonly IUnitOfWork _unitOfWork;

        public DataHelper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ShoeAuxillaryData GetAuxillaryData()
        {
            ShoeAuxillaryData shoeAuxillaryData = new(
                _unitOfWork.Context.Set<Category>().ToList(),
                _unitOfWork.Context.Set<Brand>().ToList(),
                _unitOfWork.Context.Set<Size>().ToList(),
                _unitOfWork.Context.Set<Color>().ToList()
            );
            
            return shoeAuxillaryData;
        }
    }
}
