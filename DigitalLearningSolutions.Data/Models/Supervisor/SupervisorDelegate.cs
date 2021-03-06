﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    public class SupervisorDelegate : BaseSearchableItem
    {
        public int ID { get; set; }
        public string SupervisorEmail { get; set; }
        public int? SupervisorAdminID { get; set; }
        public int CentreId { get; set; }
        public string DelegateEmail { get; set; }
        public int? CandidateID { get; set; }
        public DateTime Added { get; set; }
        public bool AddedByDelegate { get; set; }
        public DateTime NotificationSent { get; set; }
        public DateTime? Confirmed { get; set; }
        public DateTime? Removed { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? $"{FirstName} {LastName} {DelegateEmail}";
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
