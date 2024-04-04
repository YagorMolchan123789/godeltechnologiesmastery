using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTE.Mastery.ShoeStore.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }  

        public string LastName { get; set;}

        public string Country { get; set; }

        public string City { get; set; }

        public override string UserName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public User(string firstName, string lastName, string country, string city, string email, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Country = country;
            City = city;
            UserName = email;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
