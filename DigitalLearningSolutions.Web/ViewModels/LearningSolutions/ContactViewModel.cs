namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using Microsoft.AspNetCore.Html;

    public class ContactViewModel
    {
        public HtmlString ContactText { get; }

        public ContactViewModel(string contactText)
        {
            ContactText = new HtmlString(contactText);
        }
    }
}
