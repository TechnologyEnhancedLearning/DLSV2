namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Helpers;
    using System;

    public class EnrolDelegateSetCompletByDateViewModel
    {
        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public RoleProfile RoleProfile { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public OldDateValidator.ValidationResult? CompleteByValidationResult { get; set; }
    }
}
