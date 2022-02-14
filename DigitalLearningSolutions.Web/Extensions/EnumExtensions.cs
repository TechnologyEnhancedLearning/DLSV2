using DigitalLearningSolutions.Data.Enums;

namespace DigitalLearningSolutions.Web.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this SelfAssessmentResponseStatus status)
        {
            switch (status)
            {
                case SelfAssessmentResponseStatus.NotYetResponded:
                    return "Not yet responded";
                case SelfAssessmentResponseStatus.SelfAssessed:
                    return "Self-assessed";
                case SelfAssessmentResponseStatus.Verified:
                    return "Verified";
                default:
                    return null;
            }
        }
    }
}
