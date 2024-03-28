using GTE.Mastery.ShoeStore.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Dtos
{
    public class UpdateShoeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Vendor { get; set; }

        public string ImagePath { get; set; }

        public decimal Price { get; set; }

        public Gender Gender { get; set; }

        public int BrandId { get; set; }

        public int SizeId { get; set; }

        public int ColorId { get; set; }

        public int CategoryId { get; set; }
    }
}
