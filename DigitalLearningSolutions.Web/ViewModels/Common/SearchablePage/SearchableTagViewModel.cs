namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SearchableTagViewModel : FilterOptionViewModel
    {
        public SearchableTagViewModel(FilterOptionViewModel filterOption, bool hidden = false)
            : base(
                filterOption.DisplayText,
                filterOption.NewFilterToAdd,
                filterOption.TagStatus
            )
        {
            Hidden = hidden;
        }

        public bool Hidden { get; set; }

        public string NhsTagStyle()
        {
            return TagStatus switch
            {
                FilterStatus.Default => "nhsuk-tag--grey",
                FilterStatus.Warning => "nhsuk-tag--red",
                FilterStatus.Success => "nhsuk-tag--green",
                _ => string.Empty
            };
        }
    }
}
