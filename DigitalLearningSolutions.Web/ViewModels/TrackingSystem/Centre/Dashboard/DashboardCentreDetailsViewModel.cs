﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.Models;

    public class DashboardCentreDetailsViewModel
    {
        public DashboardCentreDetailsViewModel(Centre centre, string userIpAddress)
        {
            CentreName = centre.CentreName;
            CentreId = centre.CentreId;
            Region = centre.RegionName;
            ContractType = centre.ContractType;
            CentreManager = $"{centre.ContactForename} {centre.ContactSurname}";
            Email = centre.ContactEmail;
            Telephone = centre.ContactTelephone;
            BannerText = centre.BannerText;
            IpAddress = userIpAddress;
            ApprovedIps = centre.IpPrefix;
        }

        public string CentreName { get; set; }

        public int CentreId { get; set; }

        public string Region { get; set; }

        public string ContractType { get; set; }

        public string CentreManager { get; set; }

        public string? Email { get; set; }

        public string? Telephone { get; set; }

        public string? BannerText { get; set; }

        public string IpAddress { get; set; }

        public string ApprovedIps { get; set; }
    }
}
