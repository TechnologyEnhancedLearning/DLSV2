namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegateGroupLinkedFieldFilterOptions
    {
        private const string GroupName = nameof(Group.LinkedToField);

        public static readonly FilterOptionViewModel None = new FilterOptionViewModel(
            "None",
            FilteringHelper.BuildFilterValueString(GroupName, nameof(Group.LinkedToField), "0"),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel JobGroup = new FilterOptionViewModel(
            "Job group",
            FilteringHelper.BuildFilterValueString(GroupName, nameof(Group.LinkedToField), "4"),
            FilterStatus.Default
        );
    }
}
