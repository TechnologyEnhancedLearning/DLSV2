namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class NumberOfAdministratorsViewModel
    {
        public NumberOfAdministratorsViewModel(Centre centreDetails, List<AdminUser> adminUsers)
        {
            Admins = adminUsers.Count(a => a.IsCentreAdmin).ToString();
            Supervisors = adminUsers.Count(a => a.IsSupervisor).ToString();

            var trainers = adminUsers.Count(a => a.IsTrainer);
            var cmsAdministrators = adminUsers.Count(a => a.ImportOnly);
            var cmsManagers = adminUsers.Count(a => a.IsContentManager) - cmsAdministrators;
            var ccLicences = adminUsers.Count(a => a.IsContentCreator);

            Trainers = DisplayStringHelper.GenerateNumberWithLimitDisplayString(trainers, centreDetails.TrainerSpots);
            CmsAdministrators = DisplayStringHelper.GenerateNumberWithLimitDisplayString(
                cmsAdministrators,
                centreDetails.CmsAdministratorSpots
            );
            CmsManagers = DisplayStringHelper.GenerateNumberWithLimitDisplayString(
                cmsManagers,
                centreDetails.CmsManagerSpots
            );
            CcLicences = DisplayStringHelper.GenerateNumberWithLimitDisplayString(
                ccLicences,
                centreDetails.CcLicenceSpots
            );
        }

        public string Admins { get; set; }
        public string Supervisors { get; set; }
        public string Trainers { get; set; }
        public string CmsAdministrators { get; set; }
        public string CmsManagers { get; set; }
        public string CcLicences { get; set; }
    }
}
