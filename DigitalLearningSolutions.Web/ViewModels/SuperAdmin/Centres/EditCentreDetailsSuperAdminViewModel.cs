﻿namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Web.Attributes;
    using System.ComponentModel.DataAnnotations;

    public class EditCentreDetailsSuperAdminViewModel
    {
        public EditCentreDetailsSuperAdminViewModel() { }

        public EditCentreDetailsSuperAdminViewModel(Centre centre)
        {
            CentreId = centre.CentreId;
            CentreName = centre.CentreName;
            CentreTypeId = centre.CentreTypeId;
            CentreType = centre.CentreType;
            RegionName = centre.RegionName;
            CentreEmail = centre.CentreEmail;
            IpPrefix = centre.IpPrefix?.Trim();
            ShowOnMap = centre.ShowOnMap;
            RegionId = centre.RegionId;
        }

        public int CentreId { get; set; }
        [Required(ErrorMessage = "Enter a centre name")]
        [MaxLength(250, ErrorMessage = "Centre name must be 250 characters or fewer")]
        public string CentreName { get; set; }
        public int CentreTypeId { get; set; }
        public string? CentreType { get; set; }
        public int RegionId { get; set; }
        public string? RegionName { get; set; }
        [MaxLength(250, ErrorMessage = "Email must be 250 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email in the correct format, like name@example.com")]
        [NoWhitespace(ErrorMessage = "Email must not contain any whitespace characters")]
        public string? CentreEmail { get; set; }

        [RegularExpression(@"^[\d.,\s]+$", ErrorMessage = "IP Prefix can contain only digits, stops, commas and spaces")]
        public string? IpPrefix { get; set; }
        public bool ShowOnMap { get; set; }
    }
}
