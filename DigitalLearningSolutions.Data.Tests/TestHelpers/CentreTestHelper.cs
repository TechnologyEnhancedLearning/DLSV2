namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.DbModels;

    public static class CentreTestHelper
    {
        public static Centre GetDefaultCentre
        (
            int centreId = 2,
            string centreName = "North West Boroughs Healthcare NHS Foundation Trust",
            bool active = true,
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
            double longitude = -2.608441,
            double latitude = 53.428349,
            string? openingHours = "9.30am - 4.30pm",
            string? centreWebAddress = null,
            string? organisationsCovered = "Northwest Boroughs Healthcare NHS Foundation Trust",
            string? trainingVenues = "Hollins Park House\nHollins Lane\nWinwick\nWarrington WA2 8WA",
            string? otherInformation = null,
            int cmsAdministratorSpots = 5,
            int cmsManagerSpots = 0,
            int ccLicenceSpots = 0,
            int trainerSpots = 0,
            string? ipPrefix = "194.176.105",
            string? contractType = "Basic",
            int customCourses = 0,
            long serverSpaceUsed = 0,
            long serverSpaceBytes = 0
        )
        {
            return new Centre
            {
                CentreId = centreId,
                CentreName = centreName,
                Active = active,
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
                Longitude = longitude,
                Latitude = latitude,
                OpeningHours = openingHours,
                CentreWebAddress = centreWebAddress,
                OrganisationsCovered = organisationsCovered,
                TrainingVenues = trainingVenues,
                OtherInformation = otherInformation,
                CmsAdministratorSpots = cmsAdministratorSpots,
                CmsManagerSpots = cmsManagerSpots,
                CcLicenceSpots = ccLicenceSpots,
                TrainerSpots = trainerSpots,
                IpPrefix = ipPrefix,
                ContractType = contractType,
                CustomCourses = customCourses,
                ServerSpaceBytes = serverSpaceBytes,
                ServerSpaceUsed = serverSpaceUsed
            };
        }

        public static CentreRanking GetCentreRank(int rank)
        {
            return new CentreRanking
            {
                CentreId = rank,
                Ranking = rank,
                CentreName = $"Centre {rank}",
                DelegateSessionCount = 10000 - rank * 10
            };
        }
    }
}
