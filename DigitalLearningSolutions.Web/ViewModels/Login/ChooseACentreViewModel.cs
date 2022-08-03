namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.ViewModels;

    public class ChooseACentreViewModel
    {
        public ChooseACentreViewModel(
            List<ChooseACentreAccountViewModel> centreUserDetails,
            string? returnUrl,
            string? unverifiedPrimaryEmail,
            IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            CentreUserDetails = centreUserDetails;
            ReturnUrl = returnUrl;
            UnverifiedPrimaryEmail = unverifiedPrimaryEmail;
            UnverifiedCentreEmails =
                unverifiedCentreEmails.Select(uce => (uce.centreName, uce.centreSpecificEmail)).ToList();
        }

        public List<ChooseACentreAccountViewModel> CentreUserDetails { get; set; }
        public string? ReturnUrl { get; set; }
        public string? UnverifiedPrimaryEmail { get; set; }
        public List<(string centreName, string? centreSpecificEmail)> UnverifiedCentreEmails { get; set; }
    }
}
