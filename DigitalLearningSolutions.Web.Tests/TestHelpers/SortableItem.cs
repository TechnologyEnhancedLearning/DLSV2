﻿namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class SortableItem : BaseSearchableItem
    {
        public SortableItem() { }

        public SortableItem(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public string Name { get; set; } = string.Empty;

        public int Number { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? Name;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
