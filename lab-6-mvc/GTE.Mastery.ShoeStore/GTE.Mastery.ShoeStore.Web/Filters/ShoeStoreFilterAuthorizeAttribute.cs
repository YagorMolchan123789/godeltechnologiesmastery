using GTE.Mastery.ShoeStore.Domain;
using GTE.Mastery.ShoeStore.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GTE.Mastery.ShoeStore.Web.Filters
{
    public class ShoeStoreFilterAuthorizeAttribute : Attribute, IAuthorizationFilter  
    {
        private readonly string _role;

        public ShoeStoreFilterAuthorizeAttribute(string role)
        {
            _role = role;
        }

        public ShoeStoreFilterAuthorizeAttribute()
        {
            _role = RoleTypes.User.ToString();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;            

            if (!user.IsInRole(_role))
            {
                context.Result = new ForbidResult();
            }

            if (user.Identity.IsAuthenticated == false)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
