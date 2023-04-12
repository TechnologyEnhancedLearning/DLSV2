namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Data.Models.Centres;
    using Microsoft.AspNetCore.Html;

    public class ContactViewModel
    {
        public HtmlString ContactText { get; }

        public CentreSummaryForFindYourCentre CentreSummary { get; }

        public ContactViewModel(string contactText)
        {
            ContactText = new HtmlString(contactText);

        }
        public ContactViewModel(string contactText, CentreSummaryForFindYourCentre centreSummary)
        {
            ContactText = new HtmlString(contactText);
            CentreSummary = centreSummary;
        }
    }
}
