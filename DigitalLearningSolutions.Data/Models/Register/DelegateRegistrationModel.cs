﻿namespace DigitalLearningSolutions.Data.Models.Register
{
    using System;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;

    public class DelegateRegistrationModel : RegistrationModel
    {
        public DelegateRegistrationModel(
            string firstName,
            string lastName,
            string email,
            int centre,
            int jobGroup,
            string? passwordHash,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6,
            string? aliasId = null,
            DateTime? notifyDate = null
        ) : base(firstName, lastName, email, centre, jobGroup, passwordHash)
        {
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            Answer4 = answer4;
            Answer5 = answer5;
            Answer6 = answer6;
            AliasId = aliasId;
            NotifyDate = notifyDate;
        }

        public DelegateRegistrationModel(
            string firstName,
            string lastName,
            string email,
            int centre,
            int jobGroup,
            string? passwordHash
        ) : base(firstName, lastName, email, centre, jobGroup, passwordHash) { }

        public DelegateRegistrationModel(
            DelegateTableRow row,
            int centreId,
            DateTime? welcomeEmailDate
        ) : this(
            row.FirstName!,
            row.LastName!,
            row.Email!,
            centreId,
            row.JobGroupId!.Value,
            null,
            row.Answer1,
            row.Answer2,
            row.Answer3,
            row.Answer4,
            row.Answer5,
            row.Answer6,
            row.AliasId,
            welcomeEmailDate
        ) { }

        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string? Answer4 { get; set; }

        public string? Answer5 { get; set; }

        public string? Answer6 { get; set; }

        public string? AliasId { get; set; }

        public DateTime? NotifyDate { get; set; }
    }
}
