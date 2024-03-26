using FluentValidation;
using GTE.Mastery.ShoeStore.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Business.Validators
{
    public class LoginValiadator : AbstractValidator<LoginDto>
    {
        public LoginValiadator() 
        {
            RuleFor(l => l.Email).NotEmpty().NotNull().WithMessage("The Email must be filled out");

            RuleFor(l => l.Password).NotEmpty().NotNull().WithMessage("The Password must be filled out");
        }
    }
}
