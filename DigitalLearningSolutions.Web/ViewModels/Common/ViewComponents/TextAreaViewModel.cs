namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;

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
            IEnumerable<string> errorMessages,
            string? cssClass = null,
            string? hintText = null,
            int? characterCount = null
        )
        {
            var errorMessageList = errorMessages.ToList();

            Id = id;
            Class = cssClass;
            Name = name;
            Label = label;
            Value = value;
            Rows = rows;
            SpellCheck = spellCheck;
            HintText = hintText;
            CharacterCount = characterCount;
            ErrorMessages = errorMessageList;
            HasError = errorMessageList.Any();
        }

        public string Id { get; set; }
        public string? Class { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string? Value { get; set; }
        public int Rows { get; set; }
        public bool SpellCheck { get; set; }
        public string? HintText { get; set; }
        public int? CharacterCount { get; set; }
        public IEnumerable<string> ErrorMessages { get; set; }
        public bool HasError { get; set; }
    }
}
