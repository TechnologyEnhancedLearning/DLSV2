﻿namespace DigitalLearningSolutions.Web.ViewModels.ForgotPassword
{
    using System.ComponentModel.DataAnnotations;

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Enter your email address")]
        [MaxLength(100, ErrorMessage = "Email address must be 100 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        public string EmailAddress { get; set; }

        public ForgotPasswordViewModel() { }

        public ForgotPasswordViewModel(string emailAddress)
        {
            EmailAddress = emailAddress;
        }
    }
}
