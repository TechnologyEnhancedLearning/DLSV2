namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;

    public class NumberOfAdministratorsViewModel
    {
        public NumberOfAdministratorsViewModel(NumberOfAdministrators numberOfAdministrators)
        {
            Admins = numberOfAdministrators.Admins.ToString();
            Supervisors = numberOfAdministrators.Supervisors.ToString();

            Trainers = DisplayStringHelper.FormatNumberWithLimit(
                numberOfAdministrators.Trainers,
                numberOfAdministrators.TrainerSpots
            );
            CmsAdministrators = DisplayStringHelper.FormatNumberWithLimit(
                numberOfAdministrators.CmsAdministrators,
                numberOfAdministrators.CmsAdministratorSpots
            );
            CmsManagers = DisplayStringHelper.FormatNumberWithLimit(
                numberOfAdministrators.CmsManagers,
                numberOfAdministrators.CmsManagerSpots
            );
            CcLicences = DisplayStringHelper.FormatNumberWithLimit(
                numberOfAdministrators.CcLicences,
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
