namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class TermsConditionsViewModel
    {
        public readonly bool HasError;

        public TermsConditionsViewModel(
            string id,
            string name,
            string? value,
            string? errorMessage
        )
        {
            Id = id;
            Name = name;
            Value = value;
            ErrorMessage = errorMessage;
            HasError = errorMessage != null;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string? Value { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
