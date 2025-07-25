﻿namespace DigitalLearningSolutions.Data.Models.User
{
    using System;

    public class UserAccount
    {
        public int Id { get; set; }
        public string PrimaryEmail { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int JobGroupId { get; set; }
        public string JobGroupName { get; set; } = string.Empty;
        public string? ProfessionalRegistrationNumber { get; set; }
        public byte[]? ProfileImage { get; set; }
        public bool Active { get; set; }
        public DateTime? LastAccessed { get; set; }
        public int? ResetPasswordId { get; set; }
        public DateTime? TermsAgreed { get; set; }
        public int FailedLoginCount { get; set; }
        public int? EmailVerificationHashID { get; set; }

        /// <summary>
        ///     This signifies that the user has either seen the PRN fields themselves
        ///     or an admin has seen the PRN fields when editing the delegate.
        ///     This is used to distinguish whether a null ProfessionalRegistrationNumber
        ///     means they have responded No or haven't answered it yet.
        /// </summary>
        public bool HasBeenPromptedForPrn { get; set; }

        public int? LearningHubAuthId { get; set; }
        public bool HasDismissedLhLoginWarning { get; set; }
        public DateTime? EmailVerified { get; set; }
        public DateTime? DetailsLastChecked { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
