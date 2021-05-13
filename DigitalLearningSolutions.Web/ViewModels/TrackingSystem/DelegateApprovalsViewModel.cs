namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem
{
    using System;
    using System.Collections.Generic;

    public class DelegateApprovalsViewModel
    {
        public List<ApprovableDelegate> delegates { get; set; }
    }

    public class ApprovableDelegate
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateRegistered { get; set; }
    }
}
