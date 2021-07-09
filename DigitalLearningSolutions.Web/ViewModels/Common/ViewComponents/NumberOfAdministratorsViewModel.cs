namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
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
            var cmsAdministrators = adminUsers.Count(a => a.IsCmsAdministrator);
            var cmsManagers = adminUsers.Count(a => a.IsCmsManager);
            var ccLicences = adminUsers.Count(a => a.IsContentCreator);

            Trainers = DisplayStringHelper.FormatNumberWithLimit(trainers, centreDetails.TrainerSpots);
            CmsAdministrators = DisplayStringHelper.FormatNumberWithLimit(
                cmsAdministrators,
                centreDetails.CmsAdministratorSpots
            );
            CmsManagers = DisplayStringHelper.FormatNumberWithLimit(
                cmsManagers,
                centreDetails.CmsManagerSpots
            );
            CcLicences = DisplayStringHelper.FormatNumberWithLimit(
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
