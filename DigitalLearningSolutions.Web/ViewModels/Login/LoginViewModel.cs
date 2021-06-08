﻿namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        public LoginViewModel()
        {
            Username = string.Empty;
            Password = string.Empty;
            RememberMe = false;
        }

        public LoginViewModel(string? returnUrl)
        {
            Username = string.Empty;
            Password = string.Empty;
            RememberMe = false;
            ReturnUrl = returnUrl;
        }

        [Required(ErrorMessage = "Enter your email address or user ID")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
