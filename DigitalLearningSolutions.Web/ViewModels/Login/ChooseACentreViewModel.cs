namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;

    public class ChooseACentreViewModel
    {
        public ChooseACentreViewModel(List<ChooseACentreAccount> centreUserDetails)
        {
            CentreUserDetails = centreUserDetails;
        }

        public List<ChooseACentreAccount> CentreUserDetails { get; set; }
    }
}
