namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class SearchableTagViewModel : FilterOptionModel
    {
        public SearchableTagViewModel(FilterOptionModel filterOption, bool hidden = false)
            : base(
                filterOption.DisplayText,
                filterOption.FilterValue,
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
