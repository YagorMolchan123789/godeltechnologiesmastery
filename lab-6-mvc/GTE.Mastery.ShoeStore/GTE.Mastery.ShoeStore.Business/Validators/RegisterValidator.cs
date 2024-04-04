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
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly Regex _phoneNumberRegex = new Regex("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$");
        private readonly Regex _upperCaseRegex = new Regex(@"[A-Z]+");
        private readonly Regex _lowerCaseRegex = new Regex(@"[a-z]+");
        private readonly Regex _digitRegex = new Regex(@"[0-9]+");

        public RegisterValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(r => r.FirstName).NotEmpty().NotNull().WithMessage("FirstName is required")
                .MaximumLength(20).WithMessage("The length of FirstName must be not more than 20 characters");

            RuleFor(r => r.LastName).NotEmpty().NotNull().WithMessage("LastName is required")
                .MaximumLength(20).WithMessage("The length of LastName must be not more than 20 characters");

            RuleFor(r => r.Country).NotEmpty().NotNull().WithMessage("Country is required")
                .MaximumLength(20).WithMessage("The length of Country must be not more than 20 characters");

            RuleFor(r => r.City).NotEmpty().NotNull().WithMessage("City is required")
                .MaximumLength(20).WithMessage("The length of City must be not more than 20 characters");

            RuleFor(r => r.PhoneNumber).NotEmpty().NotNull().WithMessage("PhoneNumber is required")
                .Matches(_phoneNumberRegex).WithMessage("Incorrect input of PhoneNumber");

            RuleFor(r => r.Email).NotEmpty().NotNull().WithMessage("The Email must be filled out")
                .EmailAddress().WithMessage("Incorrect input of Email");

            RuleFor(r => r.Email).Must(e => !_unitOfWork.Users.UserExists(e))
                .WithMessage("The user with this Email exists already");

            RuleFor(r => r.Password).NotEmpty().NotNull().WithMessage("The Password must be filled out")
                .MinimumLength(8).WithMessage("The length of Password must be not less than 8 characters")
                .MaximumLength(16).WithMessage("The length of Password must be not more than 16 characters")
                .Matches(_upperCaseRegex).WithMessage("The Password must contain at least one uppercase letter.")
                .Matches(_lowerCaseRegex).WithMessage("The Password must contain at least one lowercase letter.")
                .Matches(_digitRegex).WithMessage("Your Password must contain at least one number."); 

            RuleFor(r => r.ConfirmPassword).NotEmpty().NotNull().WithMessage("The Password must be confirmed")
                .Equal(r => r.Password).WithMessage("The ConfirmPassword does not match Password");
        }
    }
}
