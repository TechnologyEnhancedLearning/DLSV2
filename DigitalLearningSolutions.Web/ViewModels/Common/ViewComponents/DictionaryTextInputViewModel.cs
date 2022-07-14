namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class DictionaryTextInputViewModel
    {
        public DictionaryTextInputViewModel(
            string name,
            Dictionary<string, (string, string?)> labelsAndValuesById,
            bool spellCheck,
            string? autocomplete,
            Dictionary<string, IEnumerable<string>> errorMessages,
            string? cssClass = null,
            string? hintText = null
        )
        {
            Name = name;
            Class = cssClass;
            LabelsAndValuesById = labelsAndValuesById;
            SpellCheck = spellCheck;
            Autocomplete = autocomplete;
            HintText = hintText;
            ErrorMessages = errorMessages;
        }

        public string Name { get; set; }
        public Dictionary<string, (string, string?)> LabelsAndValuesById { get; set; }
        public string? Class { get; set; }
        public bool SpellCheck { get; set; }
        public string? Autocomplete { get; set; }
        public string? HintText { get; set; }
        public Dictionary<string, IEnumerable<string>> ErrorMessages { get; set; }

        public Dictionary<string, string> IdAttributes =>
            LabelsAndValuesById.Keys.ToDictionary(key => key, key => $"{Name}_{key}");

        public Dictionary<string, string> NameAttributes =>
            LabelsAndValuesById.Keys.ToDictionary(key => key, key => $"{Name}[{key}]");
    }
}
