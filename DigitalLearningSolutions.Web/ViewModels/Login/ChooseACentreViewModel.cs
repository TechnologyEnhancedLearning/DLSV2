namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;

    public class ChooseACentreViewModel
    {
        public ChooseACentreViewModel(List<ChooseACentreAccount> centreUserDetails, string? returnUrl)
        {
            CentreUserDetails = centreUserDetails;
            ReturnUrl = returnUrl;
        }

        public List<ChooseACentreAccount> CentreUserDetails { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
