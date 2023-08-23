namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    public class PeriodOfActivity
    {
        public PeriodOfActivity(DateInformation date, int enrolments, int completions, int evaluations)
        {
            DateInformation = date;
            Enrolments = enrolments;
            Completions = completions;
            Evaluations = evaluations;
        }

        public PeriodOfActivity(DateInformation date, PeriodOfActivity? data)
        {
            DateInformation = date;
            Completions = data?.Completions ?? 0;
            Evaluations = data?.Evaluations ?? 0;
            Enrolments = data?.Enrolments ?? 0;
        }

        public DateInformation DateInformation { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Enrolments { get; set; }
    }
}
