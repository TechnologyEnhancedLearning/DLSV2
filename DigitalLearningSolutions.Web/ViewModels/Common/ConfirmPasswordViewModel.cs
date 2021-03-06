﻿namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.ComponentModel.DataAnnotations;

    public class ConfirmPasswordViewModel
    {
        [Required(ErrorMessage = "Enter a password")]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or more")]
        [MaxLength(100, ErrorMessage = "Password must be 100 characters or fewer")]
        [RegularExpression(
            @"(?=.*?[^\w\s])(?=.*?[0-9])(?=.*?[A-Za-z]).*",
            ErrorMessage = "Password must contain at least 1 letter, 1 number and 1 symbol"
        )]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Repeat your password to confirm")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and re-typed password must match")]
        public string? ConfirmPassword { get; set; }
    }
}
