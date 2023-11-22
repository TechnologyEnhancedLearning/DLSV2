namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using DigitalLearningSolutions.Web.Attributes;
    using DocumentFormat.OpenXml.Wordprocessing;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddCentreSuperAdminViewModel
    {
        public AddCentreSuperAdminViewModel() { }

        public AddCentreSuperAdminViewModel(Centre centre)
        {
            CentreEmail = centre.CentreEmail;
            CentreName = centre.CentreName;
            ContactFirstName = centre.ContactForename;
            ContactLastName = centre.ContactSurname;
            ContactEmail = centre.ContactEmail;
            ContactPhone = centre.ContactTelephone;
            CentreType = centre.CentreType;
            CentreTypeId = centre.CentreTypeId;
            RegionName = centre.RegionName;
            RegionId = centre.RegionId;
            RegistrationEmail = centre.RegistrationEmail;
            IpPrefix = centre.IpPrefix;
            AddITSPcourses = centre.AddITSPcourses;
            ShowOnMap = centre.ShowOnMap;
        }

        [MaxLength(250, ErrorMessage = "Centre name must be 250 characters or fewer")]
        [Required(ErrorMessage = "Enter a centre name")]
        public string CentreName { get; set; }

        [Required(ErrorMessage = "Select a centre type")]
        public int? CentreTypeId { get; set; }

        public string? CentreType { get; set; }

        [Required(ErrorMessage = "Select a region")]
        public int? RegionId { get; set; }

        public string? RegionName { get; set; }

        [MaxLength(250, ErrorMessage = "Email must be 250 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email in the correct format, like name@example.com")]
        [NoWhitespace(ErrorMessage = "Email must not contain any whitespace characters")]
        public string? CentreEmail { get; set; }

        [RegularExpression(@"^[\d.,\s]+$", ErrorMessage = "Ip Prefix can contain only digits, stops, commas and spaces")]
        public string? IpPrefix { get; set; }
        public bool ShowOnMap { get; set; }
        public string? ContactFirstName { get; set; }
        public string? ContactLastName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        [MaxLength(250, ErrorMessage = "Email must be 250 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email in the correct format, like name@example.com")]
        [NoWhitespace(ErrorMessage = "Email must not contain any whitespace characters")]
        public string? RegistrationEmail { get; set; }

        public bool AddITSPcourses { get; set; }

        public IEnumerable<SelectListItem> CentreTypeOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> RegionNameOptions { get; set; } = new List<SelectListItem>();
    }
}
