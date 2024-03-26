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
    public class ShoeDtoValidator : AbstractValidator<CreateEditShoeDto>
    {
        public ShoeDtoValidator() 
        {
            RuleFor(s => s.Name).NotEmpty().NotNull().WithMessage("The field Name is required");

            RuleFor(s => s.Vendor).NotEmpty().NotNull().WithMessage("The field Vendor is required");

            RuleFor(s => s.Price).GreaterThan(0).WithMessage("The field Price must be more than 0");

            RuleFor(s => s.Gender).NotNull().WithMessage("The field Gender must be checked");

            RuleFor(s => s.BrandId).NotNull().WithMessage("The field Brand is required");

            RuleFor(s => s.CategoryId).NotNull().WithMessage("The field Category is required");

            RuleFor(s => s.ColorId).NotNull().WithMessage("The field Color is required");

            RuleFor(s => s.SizeId).NotNull().WithMessage("The field SIze is required");
        }
    }
}
