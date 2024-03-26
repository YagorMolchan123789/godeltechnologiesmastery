﻿using GTE.Mastery.ShoeStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Domain.Entities
{
    public class Shoe
    {
        public int Id { get; set; }

        public string? Vendor { get; set; }

        public int BrandId { get; set; }

        public virtual Brand? Brand { get; set; }

        public int SizeId { get; set; }

        public virtual Size? Size { get; set; }

        public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        public int ColorId { get; set; }
        
        public virtual Color? Color { get; set; }

        public string? Name { get; set; }

        public string? ImagePath { get; set; }

        public Gender Gender { get; set; }

        public decimal Price { get; set; }
    }
}
