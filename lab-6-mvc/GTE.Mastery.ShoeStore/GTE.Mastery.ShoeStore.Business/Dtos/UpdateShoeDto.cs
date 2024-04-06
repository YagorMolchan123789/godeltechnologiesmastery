using GTE.Mastery.ShoeStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTE.Mastery.ShoeStore.Domain.Entities;

namespace GTE.Mastery.ShoeStore.Business.Dtos
{
    public record ShoeAuxillaryData (List<Category> Categories, List<Brand> Brands, List<Size> Sizes, List<Color> Colors);

    public class UpdateShoeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Vendor { get; set; }

        public string ImagePath { get; set; }

        public decimal Price { get; set; }

        public Gender Gender { get; set; }

        public List<Gender> Genders { get; set; }

        public int BrandId { get; set; }

        public List<Brand> Brands { get; set; }

        public int SizeId { get; set; }

        public List<Size> Sizes { get; set; }

        public int ColorId { get; set; }

        public List<Color> Colors { get; set; }

        public int CategoryId { get; set; }

        public List<Category> Categories { get; set; }
    }
}
