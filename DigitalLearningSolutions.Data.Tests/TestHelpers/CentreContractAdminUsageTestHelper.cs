namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Centres;
    using System;

    public static class CentreContractAdminUsageTestHelper
    {
        public static CentreContractAdminUsage GetDefaultNumberOfAdministrators(
            int admins = 1,
            int centreManager = 1,
            int supervisors = 2,
            int trainers = 3,
            int ccLicences = 4,
            int cmsAdministrators = 5,
            int cmsManagers = 6,
            int trainerSpots = -1,
            int ccLicenceSpots = -1,
            int cmsAdministratorSpots = -1,
            int cmsManagerSpots = -1
        )
        {
            return new CentreContractAdminUsage
            {
                AdminCount = admins,
                SupervisorCount = supervisors,
                TrainerCount = trainers,
                CcLicenceCount = ccLicences,
                CentreManagerCheckCount = centreManager,
                CmsAdministratorCount = cmsAdministrators,
                CmsManagerCount = cmsManagers,
                TrainerSpots = trainerSpots,
                CcLicenceSpots = ccLicenceSpots,
                CmsAdministratorSpots = cmsAdministratorSpots,
                CmsManagerSpots = cmsManagerSpots
            };
        }
        public static ContractInfo GetDefaultEditContractInfo(
        int CentreID = 374,
        string CentreName = "##HEE Demo Centre##",
        int ContractTypeID = 1,
        string ContractType = "Premium",
        long ServerSpaceBytesInc = 5368709120,
        long DelegateUploadSpace = 52428800,
        DateTime? ContractReviewDate = null
        )
        {
            ContractReviewDate ??= DateTime.Parse("2023-08-28 16:28:55.247");
            return new ContractInfo
            {
                CentreID = CentreID,
                CentreName = CentreName,
                ContractTypeID = ContractTypeID,
                ContractType = ContractType,
                ServerSpaceBytesInc = ServerSpaceBytesInc,
                DelegateUploadSpace = DelegateUploadSpace,
                ContractReviewDate = ContractReviewDate
            };
        }
    }
}
