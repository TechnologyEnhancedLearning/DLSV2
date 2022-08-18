namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.ViewModels;

    public class ChooseACentreViewModel
    {
        public ChooseACentreViewModel(
            List<ChooseACentreAccountViewModel> centreUserDetails,
            string? returnUrl,
            bool primaryEmailIsVerified,
            int numberOfUnverifiedCentreEmails
        )
        {
            CentreUserDetails = centreUserDetails;
            ReturnUrl = returnUrl;
            PrimaryEmailIsVerified = primaryEmailIsVerified;
            NumberOfUnverifiedCentreEmails = numberOfUnverifiedCentreEmails;
        }

        public List<ChooseACentreAccountViewModel> CentreUserDetails { get; set; }
        public string? ReturnUrl { get; set; }
        public bool PrimaryEmailIsVerified { get; set; }
        public int NumberOfUnverifiedCentreEmails { get; set; }
    }
}
