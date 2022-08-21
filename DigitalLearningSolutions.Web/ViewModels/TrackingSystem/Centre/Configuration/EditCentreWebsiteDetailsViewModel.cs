namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Web.Attributes;

    public class EditCentreWebsiteDetailsViewModel
    {
        public EditCentreWebsiteDetailsViewModel() { }

        public EditCentreWebsiteDetailsViewModel(Centre centre)
        {
            CentreTelephone = centre.CentreTelephone;
            CentreEmail = centre.CentreEmail;
            CentrePostcode = centre.CentrePostcode;
            OpeningHours = centre.OpeningHours;
            CentreWebAddress = centre.CentreWebAddress;
            OrganisationsCovered = centre.OrganisationsCovered;
            TrainingVenues = centre.TrainingVenues;
            OtherInformation = centre.OtherInformation;
        }

        [MaxLength(100, ErrorMessage = "Telephone number must be 100 characters or fewer")]
        public string? CentreTelephone { get; set; }

        [Required(ErrorMessage = "Enter an email")]
        [MaxLength(100, ErrorMessage = "Email must be 100 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email in the correct format, like name@example.com")]
        [NoWhitespace(ErrorMessage = "Email must not contain any whitespace characters")]
        public string? CentreEmail { get; set; }

        [Required(ErrorMessage = "Enter a postcode")]
        [MaxLength(10, ErrorMessage = "Postcode must be 10 characters or fewer")]
        public string? CentrePostcode { get; set; }

        [MaxLength(100, ErrorMessage = "Opening hours must be 100 characters or fewer")]
        public string? OpeningHours { get; set; }

        [MaxLength(100, ErrorMessage = "Web address must be 100 characters or fewer")]
        public string? CentreWebAddress { get; set; }

        public string? OrganisationsCovered { get; set; }

        public string? TrainingVenues { get; set; }

        public string? OtherInformation { get; set; }
    }
}
