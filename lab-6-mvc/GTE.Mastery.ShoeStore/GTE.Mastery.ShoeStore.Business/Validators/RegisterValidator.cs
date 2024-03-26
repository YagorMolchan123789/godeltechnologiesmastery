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
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator() 
        {
            RuleFor(r => r.FirstName).NotEmpty().NotNull().WithMessage("The FirstName of user must be filled out")
                .MaximumLength(20).WithMessage("The length of FirstName must be not more than 20 characters");

            RuleFor(r => r.LastName).NotEmpty().NotNull().WithMessage("The LastName of user must be filled out")
                .MaximumLength(20).WithMessage("The length of LastName must be not more than 20 characters");

            RuleFor(r => r.Country).NotEmpty().NotNull().WithMessage("The Country must be filled out");

            RuleFor(r => r.City).NotEmpty().NotNull().WithMessage("The City must be filled out");

            RuleFor(r => r.PhoneNumber).NotEmpty().NotNull().WithMessage("The PhoneNumber must be filled out")
                .Matches(new Regex("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$")).WithMessage("Incorrect input of PhoneNumber");

            RuleFor(r => r.Email).NotEmpty().NotNull().WithMessage("The Email must be filled out")
                .EmailAddress().WithMessage("Incorrect input of Email");

            RuleFor(r => r.Password).NotEmpty().NotNull().WithMessage("The Password must be filled out")
                .MinimumLength(7).WithMessage("The length of Password must be not less than 7 characters")
                .MaximumLength(16).WithMessage("The length of Password must be not more than 16 characters")
                .Matches(@"[A-Z]+").WithMessage("The Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("The Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your Password must contain at least one number."); 

            RuleFor(r => r.ConfirmPassword).NotEmpty().NotNull().WithMessage("The Password must be confirmed")
                .Equal(r => r.Password).WithMessage("The ConfirmPassword does not match Password");
        }
    }
}
