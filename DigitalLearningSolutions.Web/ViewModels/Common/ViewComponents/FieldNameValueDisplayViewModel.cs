namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class FieldNameValueDisplayViewModel
    {
        public FieldNameValueDisplayViewModel(string displayName, string value)
        {
            DisplayName = displayName;
            Value = value;
        }

        public string DisplayName { get; set; }

        public string Value { get; set; }
    }
}
