namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Helpers;
    using System;

    public class EnrolDelegateSetCompleteByDateViewModel
    {
        public EnrolDelegateSetCompleteByDateViewModel(
            SupervisorDelegateDetail supervisorDelegate,
            RoleProfile roleProfile,
            DateTime? completeByDate
        )
        {
            SupervisorDelegateDetail = supervisorDelegate;
            RoleProfile = roleProfile;
            Day = completeByDate?.Day;
            Month = completeByDate?.Month;
            Year = completeByDate?.Year;
        }

        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public RoleProfile RoleProfile { get; set; }
        public DateValidator.DateValidationResult? CompleteByValidationResult { get; set; }

        public DateTime? CompleteByDate => Day.HasValue && Month.HasValue && Year.HasValue
            ? new DateTime(Year.Value, Month.Value, Day.Value)
            : (DateTime?)null;
    }
}
