namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    using System.Collections.Generic;

    public class ChooseACentreViewModel
    {
        public ChooseACentreViewModel()
        {
            CentreUserDetails = new List<CentreUserDetails>();
        }

        public List<CentreUserDetails> CentreUserDetails { get; set; }
    }
}
