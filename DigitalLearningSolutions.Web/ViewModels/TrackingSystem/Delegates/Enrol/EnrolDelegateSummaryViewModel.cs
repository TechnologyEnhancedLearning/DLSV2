namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol
{
    using System;
    public class EnrolSummaryViewModel
    {
        public int DelegateId { get; set; }
        public int DelegateUserId { get; set; }
        public string? DelegateName { get; set; }
        public string? ActivityName { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public string? SupervisorName { get; set; }
        public string? SupervisorRoleName { get; set; }
        public bool? IsMandatory { get; set; }
        public string? ValidFor { get; set; }
        public bool? IsAutoRefresh { get; set; }
        public bool IsSelfAssessment { get; set; }
    }
}
