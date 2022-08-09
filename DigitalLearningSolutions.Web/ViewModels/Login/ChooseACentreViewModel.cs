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
            string? primaryEmailIfUnverified,
            IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)> unverifiedCentreEmails
        )
        {
            CentreUserDetails = centreUserDetails;
            ReturnUrl = returnUrl;
            PrimaryEmailIsVerified = primaryEmailIfUnverified == null;
            NumberOfUnverifiedCentreEmails = unverifiedCentreEmails.Count();
        }

        public List<ChooseACentreAccountViewModel> CentreUserDetails { get; set; }
        public string? ReturnUrl { get; set; }
        public bool PrimaryEmailIsVerified { get; set; }
        public int NumberOfUnverifiedCentreEmails { get; set; }
    }
}
