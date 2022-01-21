// QQ fix line endings before merge

using System;

namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    public class SupervisorDelegate : BaseSearchableItem
    {
        public SupervisorDelegate() { }

        public SupervisorDelegate(SupervisorDelegate supervisorDelegate)
        {
            ID = supervisorDelegate.ID;
            SupervisorEmail = supervisorDelegate.SupervisorEmail;
            SupervisorAdminID = supervisorDelegate.SupervisorAdminID;
            CentreId = supervisorDelegate.CentreId;
            DelegateEmail = supervisorDelegate.DelegateEmail;
            CandidateID = supervisorDelegate.CandidateID;
            Added = supervisorDelegate.Added;
            AddedByDelegate = supervisorDelegate.AddedByDelegate;
            NotificationSent = supervisorDelegate.NotificationSent;
            Confirmed = supervisorDelegate.Confirmed;
            Removed = supervisorDelegate.Removed;
            FirstName = supervisorDelegate.FirstName;
            LastName = supervisorDelegate.LastName;
            SearchableNameOverrideForFuzzySharp = supervisorDelegate.SearchableNameOverrideForFuzzySharp;
        }

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
