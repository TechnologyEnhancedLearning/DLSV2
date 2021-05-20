namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class TextAreaViewModel
    {
        public TextAreaViewModel
        (
            string id,
            string name,
            string label,
            string? value,
            int rows,
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
            Rows = rows;
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
        public int Rows { get; set; }
        public bool SpellCheck { get; set; }
        public string? Autocomplete { get; set; }
        public string? HintText { get; set; }
        public string? ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
