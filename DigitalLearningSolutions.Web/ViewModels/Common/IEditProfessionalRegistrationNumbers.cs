namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public interface IEditProfessionalRegistrationNumbers
    {
        public string? ProfessionalRegistrationNumber { get; set; }

        public bool? HasProfessionalRegistrationNumber { get; set; }

        public bool IsSelfRegistrationOrEdit { get; set; }
    }
}
