using DigitalLearningSolutions.Data.Models.TrackingSystem;

namespace DigitalLearningSolutions.Data.Models.PlatformReports
{
    public class SelfAssessmentActivityInPeriod
    {
        public SelfAssessmentActivityInPeriod(DateInformation date, int enrolments, int completions)
        {
            DateInformation = date;
            Enrolments = enrolments;
            Completions = completions;
        }

        public SelfAssessmentActivityInPeriod(DateInformation date, SelfAssessmentActivityInPeriod? data)
        {
            DateInformation = date;
            Completions = data?.Completions ?? 0;
            Enrolments = data?.Enrolments ?? 0;
        }

        public DateInformation DateInformation { get; set; }
        public int Completions { get; set; }
        public int Enrolments { get; set; }
    }
}
