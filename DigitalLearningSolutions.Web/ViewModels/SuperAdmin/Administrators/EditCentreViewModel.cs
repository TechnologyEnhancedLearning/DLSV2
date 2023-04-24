using DigitalLearningSolutions.Data.Models.User;
using DigitalLearningSolutions.Web.ViewModels.Common;

namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Administrators
{
    public class EditCentreViewModel : AdminRolesViewModel
    {
        public EditCentreViewModel(
           AdminUser user,
           int centreId
       ) : base(user, centreId)
        {
            AdminId = user.Id;
            CentreName = user.CentreName;
            AdminCentreId = centreId;
        }
        public int AdminCentreId { get; set; }
        public int AdminId { get; set; }
        public string CentreName { get; set; }
        public string? SearchString { get; set; }
        public string? ExistingFilterString { get; set; }
        public int Page { get; set; }
    }
}
