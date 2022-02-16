using DigitalLearningSolutions.Data.Enums;

namespace DigitalLearningSolutions.Web.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this SelfAssessmentCompetencyFilter status)
        {
            switch (status)
            {
                case SelfAssessmentCompetencyFilter.NotYetResponded:
                    return "Not yet responded";
                case SelfAssessmentCompetencyFilter.SelfAssessed:
                    return "Self-assessed";
                case SelfAssessmentCompetencyFilter.Verified:
                    return "Verified";
                case SelfAssessmentCompetencyFilter.MeetingRequirements:
                    return "Meeting requirements";
                case SelfAssessmentCompetencyFilter.PartiallyMeetingRequirements:
                    return "Partially meeting requirements";
                case SelfAssessmentCompetencyFilter.NotMeetingRequirements:
                    return "Not meeting requirements";
                default:
                    return null;
            }
        }
    }
}
