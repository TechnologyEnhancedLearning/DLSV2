namespace DigitalLearningSolutions.Data.Models.Centres
{
    public class CentreSummaryForMap
    {
        public int Id { get; set; }

        public string CentreName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string? Telephone { get; set; }

        public string? Email { get; set; }

        public string? WebUrl { get; set; }

        public string? Hours { get; set; }

        public string? TrustsCovered { get; set; }

        public string? TrainingLocations { get; set; }

        public string? GeneralInfo { get; set; }

        public bool SelfRegister { get; set; }
    }
}
