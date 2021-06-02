﻿namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models;

    public static class CentreTestHelper
    {
        public static Centre GetDefaultCentre
        (
            int centreId = 2,
            string centreName = "North West Boroughs Healthcare NHS Foundation Trust",
            int regionId = 5,
            string regionName = "North West",
            string? notifyEmail = "notify@test.com",
            string? bannerText = "xxxxxxxxxxxxxxxxxxxx",
            byte[]? signatureImage = null,
            byte[]? centreLogo = null,
            string? contactForename = "xxxxx",
            string? contactSurname = "xxxx",
            string? contactEmail = "nybwhudkra@ic.vs",
            string? contactTelephone = "xxxxxxxxxxxx",
            string? centreTelephone = "01925 664457",
            string? centreEmail = "5bp.informaticstraining.5bp.nhs.uk",
            string? centrePostcode = "WA2 8WA",
            bool showCentreOnMap = true,
            string? openingHours = "9.30am - 4.30pm",
            string? centreWebAddress = null,
            string? organisationsCovered = "Northwest Boroughs Healthcare NHS Foundation Trust",
            string? trainingVenues = "Hollins Park House\nHollins Lane\nWinwick\nWarrington WA2 8WA",
            string? otherInformation = null,
            string? ipPrefix = "194.176.105",
            int contractTypeId = 1,
            string? contractType = "Basic"
        )
        {
            return new Centre
            {
                CentreId = centreId,
                CentreName = centreName,
                RegionId = regionId,
                RegionName = regionName,
                NotifyEmail = notifyEmail,
                BannerText = bannerText,
                SignatureImage = signatureImage,
                CentreLogo = centreLogo,
                ContactForename = contactForename,
                ContactSurname = contactSurname,
                ContactEmail = contactEmail,
                ContactTelephone = contactTelephone,
                CentreTelephone = centreTelephone,
                CentreEmail = centreEmail,
                CentrePostcode = centrePostcode,
                ShowOnMap = showCentreOnMap,
                OpeningHours = openingHours,
                CentreWebAddress = centreWebAddress,
                OrganisationsCovered = organisationsCovered,
                TrainingVenues = trainingVenues,
                OtherInformation = otherInformation,
                IpPrefix = ipPrefix,
                ContractTypeId = contractTypeId,
                ContractType = contractType
            };
        }
    }
}
