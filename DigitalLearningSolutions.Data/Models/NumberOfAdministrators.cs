namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    public class NumberOfAdministrators
    {
        public NumberOfAdministrators() {}

        public NumberOfAdministrators(Centre centreDetails, List<AdminUser> adminUsers)
        {
            Admins = adminUsers.Count(a => a.IsCentreAdmin);
            Supervisors = adminUsers.Count(a => a.IsSupervisor);
            Trainers = adminUsers.Count(a => a.IsTrainer);
            CmsAdministrators = adminUsers.Count(a => a.IsCmsAdministrator);
            CmsManagers = adminUsers.Count(a => a.IsCmsManager);
            CcLicences = adminUsers.Count(a => a.IsContentCreator);
            TrainerSpots = centreDetails.TrainerSpots;
            CmsAdministratorSpots = centreDetails.CmsAdministratorSpots;
            CmsManagerSpots = centreDetails.CmsManagerSpots;
            CcLicenceSpots = centreDetails.CcLicenceSpots;
        }

        public int Admins { get; set; }
        public int Supervisors { get; set; }
        public int Trainers { get; set; }
        public int CmsAdministrators { get; set; }
        public int CmsManagers { get; set; }
        public int CcLicences { get; set; }
        public int TrainerSpots { get; set; }
        public int CmsAdministratorSpots { get; set; }
        public int CmsManagerSpots { get; set; }
        public int CcLicenceSpots { get; set; }

        public bool TrainersAtOrOverLimit => TrainerSpots != -1 && Trainers >= TrainerSpots;
        public bool CmsAdministratorsAtOrOverLimit => CmsAdministratorSpots != -1 && CmsAdministrators >= CmsAdministratorSpots;
        public bool CmsManagersAtOrOverLimit => CmsManagerSpots != -1 && CmsManagers >= CmsManagerSpots;
        public bool CcLicencesAtOrOverLimit => CcLicenceSpots != -1 && CcLicences >= CcLicenceSpots;

    }
}
