using FluentValidation;
using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Validators
{
    public class UpdateShoeDtoValidator : AbstractValidator<UpdateShoeDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateShoeDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(s => s.Name).NotEmpty().NotNull().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("The length of Name must be not more than 50 characters");

            RuleFor(s => s.Vendor).NotEmpty().NotNull().WithMessage("Vendor number is required");

            RuleFor(s => s.Price).GreaterThan(0).WithMessage("Price must be more than 0");

            RuleFor(s => s.Gender).NotNull().WithMessage("Gender must be set");

            RuleFor(s => s.BrandId).NotEqual(0).WithMessage("Brand must be set");

            RuleFor(s => s.CategoryId).NotEqual(0).WithMessage("Category must be set");

            RuleFor(s => s.ColorId).NotEqual(0).WithMessage("Color must be set");

            RuleFor(s => s.SizeId).NotEqual(0).WithMessage("Size must be set");

            RuleFor(s => s).Must(s => _unitOfWork.Shoes.IsUnique(s.Id, s.Name, s.SizeId, s.ColorId))
                .WithMessage("The shoe with such name, size, color already exists");

        }
    }
}
