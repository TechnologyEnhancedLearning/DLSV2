using DigitalLearningSolutions.Data.Models.User;
using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    public class DeactivateAdminViewModel
    {
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public int UserId { get; private set; }
        public bool Confirm { get; set; }

        public DeactivateAdminViewModel() { }
 
        public DeactivateAdminViewModel(AdminUser user)
        {
            if (user != null)
            {
                UserId = user.Id;
                FullName = user.FullName;
                EmailAddress = user.EmailAddress;
            }
        }
    }
}
