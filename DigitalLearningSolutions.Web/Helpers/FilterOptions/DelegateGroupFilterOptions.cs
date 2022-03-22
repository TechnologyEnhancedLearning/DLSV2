namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegateGroupLinkedFieldFilterOptions
    {
        private const string GroupName = nameof(Group.LinkedToField);

        public static readonly FilterOptionModel None = new FilterOptionModel(
            "None",
            FilteringHelper.BuildFilterValueString(GroupName, nameof(Group.LinkedToField), "0"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel JobGroup = new FilterOptionModel(
            "Job group",
            FilteringHelper.BuildFilterValueString(GroupName, nameof(Group.LinkedToField), "4"),
            FilterStatus.Default
        );
    }
}
