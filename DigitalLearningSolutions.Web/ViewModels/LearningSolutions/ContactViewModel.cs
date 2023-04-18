namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Data.Models.Centres;
    using Microsoft.AspNetCore.Html;

    public class ContactViewModel
    {
        public HtmlString ContactText { get; }

        public CentreSummaryForContactDisplay CentreSummary { get; }

        public ContactViewModel(string contactText)
        {
            ContactText = new HtmlString(contactText);

        }
        public ContactViewModel(string contactText, CentreSummaryForContactDisplay centreSummary)
        {
            ContactText = new HtmlString(contactText);
            CentreSummary = centreSummary;
        }
    }
}
