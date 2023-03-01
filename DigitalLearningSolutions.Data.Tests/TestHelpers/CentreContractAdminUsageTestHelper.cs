namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models;

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
    }
}
