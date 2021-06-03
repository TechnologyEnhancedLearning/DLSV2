namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public class NumberOfAdministratorsViewModel
    {
        public int Admins { get; set; }
        public int Supervisors { get; set; }
        public int Trainers { get; set; }
        public int TrainerSpotsAvailable { get; set; }
        public int CmsAdministrators { get; set; }
        public int CmsAdministratorSpotsAvailable { get; set; }
        public int CmsManagers { get; set; }
        public int CmsManagerSpotsAvailable { get; set; }
        public int CcLicences { get; set; }
        public int CcLicencesAvailable { get; set; }

        public NumberOfAdministratorsViewModel(Centre centreDetails, List<AdminUser> adminUsers)
        {
            TrainerSpotsAvailable = centreDetails.Trainers;
            CmsAdministratorSpotsAvailable = centreDetails.CmsAdministrators;
            CmsManagerSpotsAvailable = centreDetails.CmsManagers;
            CcLicencesAvailable = centreDetails.CcLicences;

            Admins = adminUsers.Count(a => a.IsCentreAdmin);
            Supervisors = adminUsers.Count(a => a.IsSupervisor);
            Trainers = adminUsers.Count(a => a.IsTrainer);
            CmsAdministrators = adminUsers.Count(a => a.ImportOnly);
            CmsManagers = adminUsers.Count(a => a.IsContentManager) - CmsAdministrators;
            CcLicences = adminUsers.Count(a => a.IsContentCreator);
        }

    }
}
