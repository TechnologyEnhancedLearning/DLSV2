namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;

    public class ChooseACentreViewModel
    {
        public ChooseACentreViewModel(
            List<ChooseACentreAccountViewModel> centreUserDetails,
            string? returnUrl,
            bool primaryEmailIsVerified,
            List<(int centreId, string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            CentreUserDetails = centreUserDetails;
            ReturnUrl = returnUrl;
            PrimaryEmailIsVerified = primaryEmailIsVerified;
            UnverifiedCentreEmails = unverifiedCentreEmails;
        }

        public List<ChooseACentreAccountViewModel> CentreUserDetails { get; set; }
        public string? ReturnUrl { get; set; }
        public bool PrimaryEmailIsVerified { get; set; }
        public List<(int centreId, string centreName, string? centreSpecificEmail)> UnverifiedCentreEmails { get; set; }
    }
}
