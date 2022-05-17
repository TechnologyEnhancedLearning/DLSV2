namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;

    public class ChooseACentreViewModel
    {
        public ChooseACentreViewModel(List<CentreUserDetails> centreUserDetails)
        {
            CentreUserDetails = centreUserDetails;
        }

        public List<CentreUserDetails> CentreUserDetails { get; set; }
    }
}
