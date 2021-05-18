namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class TextInputViewModel
    {
        public TextInputViewModel
        (
            string id,
            string name,
            string label,
            string? value,
            string type,
            bool spellCheck,
            string? autocomplete,
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
            Value = value;
            Type = type;
            SpellCheck = spellCheck;
            Autocomplete = autocomplete;
            HintText = hintText;
            ErrorMessage = errorMessage;
            HasError = hasError;
        }

        public string Id { get; set; }
        public string? Class { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string? Value { get; set; }
        public string Type { get; set; }
        public bool SpellCheck { get; set; }
        public string? Autocomplete { get; set; }
        public string? HintText { get; set; }
        public string? ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
