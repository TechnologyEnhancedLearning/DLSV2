namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models;

    public static class NumberOfAdministratorsTestHelper
    {
        public static NumberOfAdministrators GetDefaultNumberOfAdministrators(
            int admins = 1,
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
            return new NumberOfAdministrators
            {
                Admins = admins,
                Supervisors = supervisors,
                Trainers = trainers,
                CcLicences = ccLicences,
                CmsAdministrators = cmsAdministrators,
                CmsManagers = cmsManagers,
                TrainerSpots = trainerSpots,
                CcLicenceSpots = ccLicenceSpots,
                CmsAdministratorSpots = cmsAdministratorSpots,
                CmsManagerSpots = cmsManagerSpots
            };
        }
    }
}
