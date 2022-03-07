namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.User;

    public class CentreContractAdminUsage
    {
        public CentreContractAdminUsage() {}

        public CentreContractAdminUsage(Centre centreDetails, List<AdminUser> adminUsers)
        {
            AdminCount = adminUsers.Count(a => a.IsCentreAdmin);
            SupervisorCount = adminUsers.Count(a => a.IsSupervisor);
            NominatedSupervisorCount = adminUsers.Count(a => a.IsNominatedSupervisor);
            TrainerCount = adminUsers.Count(a => a.IsTrainer);
            CmsAdministratorCount = adminUsers.Count(a => a.IsCmsAdministrator);
            CmsManagerCount = adminUsers.Count(a => a.IsCmsManager);
            CcLicenceCount = adminUsers.Count(a => a.IsContentCreator);
            TrainerSpots = centreDetails.TrainerSpots;
            CmsAdministratorSpots = centreDetails.CmsAdministratorSpots;
            CmsManagerSpots = centreDetails.CmsManagerSpots;
            CcLicenceSpots = centreDetails.CcLicenceSpots;
        }

        public int AdminCount { get; set; }
        public int SupervisorCount { get; set; }
        public int NominatedSupervisorCount { get; set; }
        public int TrainerCount { get; set; }
        public int CmsAdministratorCount { get; set; }
        public int CmsManagerCount { get; set; }
        public int CcLicenceCount { get; set; }
        public int TrainerSpots { get; set; }
        public int CmsAdministratorSpots { get; set; }
        public int CmsManagerSpots { get; set; }
        public int CcLicenceSpots { get; set; }

        public bool TrainersAtOrOverLimit => TrainerSpots != -1 && TrainerCount >= TrainerSpots;
        public bool CmsAdministratorsAtOrOverLimit => CmsAdministratorSpots != -1 && CmsAdministratorCount >= CmsAdministratorSpots;
        public bool CmsManagersAtOrOverLimit => CmsManagerSpots != -1 && CmsManagerCount >= CmsManagerSpots;
        public bool CcLicencesAtOrOverLimit => CcLicenceSpots != -1 && CcLicenceCount >= CcLicenceSpots;

    }
}
