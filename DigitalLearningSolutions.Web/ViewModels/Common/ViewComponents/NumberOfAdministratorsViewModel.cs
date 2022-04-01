namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;

    public class NumberOfAdministratorsViewModel
    {
        public NumberOfAdministratorsViewModel(CentreContractAdminUsage numberOfAdministrators)
        {
            Admins = numberOfAdministrators.AdminCount.ToString();
            Supervisors = numberOfAdministrators.SupervisorCount.ToString();

            Trainers = DisplayStringHelper.FormatNumberWithLimit(
                numberOfAdministrators.TrainerCount,
                numberOfAdministrators.TrainerSpots
            );
            CmsAdministrators = DisplayStringHelper.FormatNumberWithLimit(
                numberOfAdministrators.CmsAdministratorCount,
                numberOfAdministrators.CmsAdministratorSpots
            );
            CmsManagers = DisplayStringHelper.FormatNumberWithLimit(
                numberOfAdministrators.CmsManagerCount,
                numberOfAdministrators.CmsManagerSpots
            );
            CcLicences = DisplayStringHelper.FormatNumberWithLimit(
                numberOfAdministrators.CcLicenceCount,
                numberOfAdministrators.CcLicenceSpots
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
