using DigitalLearningSolutions.Data.Enums;

namespace DigitalLearningSolutions.Web.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this DlsRole role)
        {
            switch (role)
            {
                case DlsRole.NominatedSupervisor:
                    return "Nominated supervisor";
                default:
                    return role.ToString();
            }
        }

        public static string GetDescription(this SelfAssessmentCompetencyFilter status, bool isSupervisorResultReview = false)
        {
            switch (status)
            {
                case SelfAssessmentCompetencyFilter.RequiresSelfAssessment:
                    return "Requires self assessment";
                case SelfAssessmentCompetencyFilter.SelfAssessed:
                    return "Self-assessed" + (isSupervisorResultReview ? " (confirmation not yet requested)" : "");
                case SelfAssessmentCompetencyFilter.ConfirmationRequested:
                    return "Confirmation requested";
                case SelfAssessmentCompetencyFilter.ConfirmationRejected:
                    return "Confirmation rejected";
                case SelfAssessmentCompetencyFilter.Verified:
                    return "Confirmed";
                case SelfAssessmentCompetencyFilter.MeetingRequirements:
                    return "Meeting requirements";
                case SelfAssessmentCompetencyFilter.PartiallyMeetingRequirements:
                    return "Partially meeting requirements";
                case SelfAssessmentCompetencyFilter.NotMeetingRequirements:
                    return "Not meeting requirements";
                default:
                    return status.ToString();
            }
        }
    }
}
