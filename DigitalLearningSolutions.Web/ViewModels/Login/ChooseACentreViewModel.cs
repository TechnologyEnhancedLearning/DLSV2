namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;

    public class ChooseACentreViewModel
    {
        public ChooseACentreViewModel(
            List<ChooseACentreAccountViewModel> centreUserDetails,
            string? returnUrl,
            bool primaryEmailIsUnverified,
            List<string> unverifiedCentreEmails
        )
        {
            CentreUserDetails = centreUserDetails;
            ReturnUrl = returnUrl;
            PrimaryEmailIsUnverified = primaryEmailIsUnverified;
            UnverifiedCentreEmails = unverifiedCentreEmails;
        }

        public List<ChooseACentreAccountViewModel> CentreUserDetails { get; set; }
        public string? ReturnUrl { get; set; }
        public bool PrimaryEmailIsUnverified { get; set; }
        public List<string> UnverifiedCentreEmails { get; set; }
    }
}
