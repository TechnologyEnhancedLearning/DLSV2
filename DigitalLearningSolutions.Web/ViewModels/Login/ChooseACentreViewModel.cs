namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;

    public class ChooseACentreViewModel
    {
        public ChooseACentreViewModel(List<CentreUserDetails> centreUserDetails, bool adminAccountsAreLocked)
        {
            CentreUserDetails = centreUserDetails;
            AdminAccountsAreLocked = adminAccountsAreLocked;
        }

        public List<CentreUserDetails> CentreUserDetails { get; set; }
        public bool AdminAccountsAreLocked { get; set; }
    }
}
