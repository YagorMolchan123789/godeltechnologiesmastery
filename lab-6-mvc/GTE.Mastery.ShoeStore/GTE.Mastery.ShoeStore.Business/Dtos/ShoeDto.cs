using GTE.Mastery.ShoeStore.Domain.Entities;
using GTE.Mastery.ShoeStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Dtos
{
    public class ShoeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Vendor { get; set; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        public int SizeId { get; set; }

        public Size Size { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }  

        public int ColorId { get; set; }

        public Color Color { get; set; }

        public string ImagePath { get; set; }
        
        public Gender Gender { get; set; }

        public decimal Price { get; set; }
    }
}
