using AutoMapper;
using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Profiles
{
    public class ShoeProfile : Profile
    {
        public ShoeProfile()
        {
            CreateMap<Shoe, ShoeDto>();

            CreateMap<Shoe, UpdateShoeDto>();
        }
    }
}
