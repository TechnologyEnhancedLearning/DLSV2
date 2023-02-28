using DigitalLearningSolutions.Data.Models.User;
using DigitalLearningSolutions.Web.Attributes;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace DigitalLearningSolutions.Web.Attributes
{
    public class UserNameAttribute : ValidationAttribute
    {
        private readonly string? errorMessage;

        public UserNameAttribute(string? errorMessage = null)
        {
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            string userForename = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserForename")?.Value;
            string userSurname = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserSurname")?.Value;

            string passwordLowercase = value.ToString().ToLower();
            userForename = userForename.ToLower();
            userSurname = userSurname.ToLower();

            if (passwordLowercase.Contains(userForename) || passwordLowercase.Contains(userSurname))
            {
                return new ValidationResult(this.errorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
