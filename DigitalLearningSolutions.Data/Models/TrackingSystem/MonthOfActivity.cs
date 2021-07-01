namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    public class MonthOfActivity
    {
        public MonthOfActivity()
        {
            Year = 0;
            Month = 0;
            Completions = 0;
            Evaluations = 0;
            Registrations = 0;
        }

        public MonthOfActivity((int Month, int Year) slot, MonthOfActivity? data)
        {
            Year = slot.Year;
            Month = slot.Month;
            Completions = data?.Completions ?? 0;
            Evaluations = data?.Evaluations ?? 0;
            Registrations = data?.Registrations ?? 0;
        }

        public int Year { get; set; }
        public int Month { get; set; }
        public int Completions { get; set; }
        public int Evaluations { get; set; }
        public int Registrations { get; set; }
    }
}
