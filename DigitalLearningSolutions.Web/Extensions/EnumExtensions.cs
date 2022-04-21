﻿using DigitalLearningSolutions.Data.Enums;

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

        public static string GetDescription(this SelfAssessmentCompetencyFilter status)
        {
            switch (status)
            {
                case SelfAssessmentCompetencyFilter.NotYetResponded:
                    return "Not yet responded";
                case SelfAssessmentCompetencyFilter.SelfAssessed:
                    return "Self-assessed";
                case SelfAssessmentCompetencyFilter.Verified:
                    return "Confirmed";
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

        public static bool IsRequirementsFilter(this SelfAssessmentCompetencyFilter filter)
        {
            return filter == SelfAssessmentCompetencyFilter.MeetingRequirements
                || filter == SelfAssessmentCompetencyFilter.PartiallyMeetingRequirements
                || filter == SelfAssessmentCompetencyFilter.NotMeetingRequirements;
        }

        public static bool IsResponseStatusFilter(this SelfAssessmentCompetencyFilter filter)
        {
            return filter == SelfAssessmentCompetencyFilter.NotYetResponded
                || filter == SelfAssessmentCompetencyFilter.SelfAssessed
                || filter == SelfAssessmentCompetencyFilter.Verified;
        }
    }
}
