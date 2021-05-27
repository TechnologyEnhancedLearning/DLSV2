namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models;

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

        [MaxLength(100, ErrorMessage = "Enter a telephone number that is no more than 100 characters.")]
        public string? CentreTelephone { get; set; }

        [Required(ErrorMessage = "Enter an email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(100, ErrorMessage = "Enter an email address that is no more than 100 characters.")]
        public string? CentreEmail { get; set; }

        [Required(ErrorMessage = "Enter a postcode.")]
        [MaxLength(10, ErrorMessage = "Enter a postcode that is no more than 10 characters.")]
        public string? CentrePostcode { get; set; }

        [MaxLength(100, ErrorMessage = "Enter no more than 100 characters for the opening hours.")]
        public string? OpeningHours { get; set; }

        [MaxLength(100, ErrorMessage = "Enter a web address that is no more than 100 characters.")]
        public string? CentreWebAddress { get; set; }

        public string? OrganisationsCovered { get; set; }

        public string? TrainingVenues { get; set; }

        public string? OtherInformation { get; set; }
    }
}
