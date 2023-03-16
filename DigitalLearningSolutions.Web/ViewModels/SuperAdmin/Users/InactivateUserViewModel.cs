using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    public class InactivateUserViewModel
    {
        public int UserId { get; set; }

        public string DisplayName { get; set; }

        public bool IsChecked { get; set; }

        public bool Error { get; set; }

        public string SearchString { get; set; }

        public string ExistingFilterString { get; set; }

        public int Page { get; set; }
    }
}
