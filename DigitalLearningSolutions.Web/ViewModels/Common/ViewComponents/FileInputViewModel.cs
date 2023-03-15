namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class FileInputViewModel
    {
        public FileInputViewModel
        (
            string id,
            string name,
            string label,
            string? cssClass = null,
            string? hintText = null,
            string? errorMessage = null,
            bool hasError = false
        )
        {
            Id = id;
            Class = cssClass;
            Name = name;
            Label = label;
            HintText = hintText;
            ErrorMessage = errorMessage;
            HasError = hasError;
        }

        public string Id { get; set; }
        public string? Class { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string? HintText { get; set; }
        public string? ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
