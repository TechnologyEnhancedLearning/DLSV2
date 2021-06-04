namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public class NumberOfAdministratorsViewModel
    {
        public string Admins { get; set; }
        public string Supervisors { get; set; }
        public string Trainers { get; set; }
        public string CmsAdministrators { get; set; }
        public string CmsManagers { get; set; }
        public string CcLicences { get; set; }

        public NumberOfAdministratorsViewModel(Centre centreDetails, List<AdminUser> adminUsers)
        {
            Admins = adminUsers.Count(a => a.IsCentreAdmin).ToString();
            Supervisors = adminUsers.Count(a => a.IsSupervisor).ToString();

            var trainers = adminUsers.Count(a => a.IsTrainer);
            var cmsAdministrators = adminUsers.Count(a => a.ImportOnly);
            var cmsManagers = adminUsers.Count(a => a.IsContentManager) - cmsAdministrators;
            var ccLicences = adminUsers.Count(a => a.IsContentCreator);

            Trainers = GenerateDisplayString(trainers, centreDetails.TrainerSpots);
            CmsAdministrators = GenerateDisplayString(cmsAdministrators, centreDetails.CmsAdministratorSpots);
            CmsManagers = GenerateDisplayString(cmsManagers, centreDetails.CmsManagerSpots);
            CcLicences = GenerateDisplayString(ccLicences, centreDetails.CcLicenceSpots);
        }

        private string GenerateDisplayString(int number, int limit)
        {
            return limit == -1 ? number.ToString() : number + " / " + limit;
        }
    }
}
