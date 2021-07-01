namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using System.Linq;

    public class CheckboxViewModel
    {
        public CheckboxViewModel(
            string id,
            string name,
            string label,
            bool value,
            IEnumerable<string> errorMessages,
            string? cssClass = null,
            string? hintText = null
        )
        {
            var errorMessageList = errorMessages.ToList();

            Id = id;
            Class = cssClass;
            Name = name;
            Label = label;
            Value = value;
            HintText = hintText;
            ErrorMessages = errorMessageList;
            HasError = errorMessageList.Any();
        }

        public string Id { get; set; }
        public string? Class { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool Value { get; set; }
        public string? HintText { get; set; }
        public IEnumerable<string> ErrorMessages { get; set; }
        public readonly bool HasError;
    }
}
