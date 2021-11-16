namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;

    public class ConfirmDeleteGroupViewModel 
    {
        public string GroupLabel { get; set; }
        public int DelegateCount { get; set; }
        public int CourseCount { get; set; }

        [BooleanMustBeTrue(ErrorMessage = "Confirm you wish to delete this group")]
        public bool Confirm { get; set; }
        public bool DeleteEnrolments { get; set; }
    }
}
