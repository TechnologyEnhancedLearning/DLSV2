namespace DigitalLearningSolutions.Web.Models
{
    public class DelegateRegistrationByCentreData : DelegateRegistrationData
    {
        public DelegateRegistrationByCentreData() { }
        public DelegateRegistrationByCentreData(int centreId) : base(centreId) { }
        public string? Alias { get; set; }
    }
}
