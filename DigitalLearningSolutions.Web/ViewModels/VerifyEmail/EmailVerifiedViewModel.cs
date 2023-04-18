namespace DigitalLearningSolutions.Web.ViewModels.VerifyEmail
{
    public class EmailVerifiedViewModel
    {
        public EmailVerifiedViewModel(
            int? centreIdIfEmailIsForUnapprovedDelegate
        )
        {
            CentreIdIfEmailIsForUnapprovedDelegate = centreIdIfEmailIsForUnapprovedDelegate;
        }

        public int? CentreIdIfEmailIsForUnapprovedDelegate { get; set; }
    }
}
