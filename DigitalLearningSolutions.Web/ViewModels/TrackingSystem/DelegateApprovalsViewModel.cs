namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class DelegateApprovalsViewModel
    {
        public DelegateApprovalsViewModel(IEnumerable<ApprovableDelegate> delegates)
        {
            Delegates = delegates;
        }

        public IEnumerable<ApprovableDelegate> Delegates { get; set; }
    }

    public class ApprovableDelegate
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateRegistered { get; set; }
        public string JobGroup { get; set; }
        public IEnumerable<CustomFieldViewModel> CustomFields { get; set; }
    }
}
