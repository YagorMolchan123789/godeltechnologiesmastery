using FluentValidation;
using GTE.Mastery.ShoeStore.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Validators
{
    public class ShoeDtoValidator : AbstractValidator<UpdateShoeDto>
    {
        public ShoeDtoValidator() 
        {
            RuleFor(s => s.Name).NotEmpty().NotNull().WithMessage("Name is required");

            RuleFor(s => s.Vendor).NotEmpty().NotNull().WithMessage("Vendor is required");

            RuleFor(s => s.Price).GreaterThan(0).WithMessage("Price must be more than 0");

            RuleFor(s => s.Gender).NotNull().WithMessage("Gender must be set");

            RuleFor(s => s.BrandId).NotNull().WithMessage("Brand is required");

            RuleFor(s => s.CategoryId).NotNull().WithMessage("Category is required");

            RuleFor(s => s.ColorId).NotNull().WithMessage("Color is required");

            RuleFor(s => s.SizeId).NotNull().WithMessage("Size is required");
        }
    }
}
