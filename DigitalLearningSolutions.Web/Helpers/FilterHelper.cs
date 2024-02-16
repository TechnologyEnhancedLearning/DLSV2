using DigitalLearningSolutions.Data.Helpers;
using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Helpers
{
    public static class FilterHelper
    {
        public static string? RemoveNonExistingPromptFilters(List<FilterModel> availableFilters, string existingFilterString)
        {
            if (existingFilterString != null && existingFilterString.Contains("Answer"))
            {
                if (availableFilters.Where(x => x.FilterGroupKey == "prompts").ToList().Any())
                {
                    var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();
                    var existingPromptFilters = existingFilterString!.Split(FilteringHelper.FilterSeparator).Where(filter => filter.Contains("Answer")).ToList();

                    foreach (var existingPromptFilter in existingPromptFilters)
                    {
                        bool isFound = false;
                        var splitFilter = existingPromptFilter.Split(FilteringHelper.Separator);
                        var filterHeaderArr = splitFilter[0].SkipWhile(c => c != '(').Skip(1).TakeWhile(c => c != ')').ToArray();
                        var filterHeader = string.Join("", filterHeaderArr); //prompt header
                        var filterOptionText = splitFilter[2] == FilteringHelper.EmptyValue ?
                                                            "No option selected" : splitFilter[2]; //prompt option text
                        var promptDbText = splitFilter[1]; //filter db text eg. Answer1

                        var availableFilterOptions = availableFilters.Where(x => x.FilterGroupKey == "prompts" && x.FilterName == filterHeader)
                                                                     .Select(o => o.FilterOptions).ToList();

                        foreach (var filterOption in availableFilterOptions)
                        {
                            if (filterOption.Any(x => x.DisplayText.Contains(filterOptionText)))
                            {
                                var filter = filterOption.Where(x => x.DisplayText.Contains(filterOptionText)).ToList().Select(x => x.FilterValue).FirstOrDefault();
                                if (!filter.Contains(promptDbText))
                                { //when prompt filter header and selected option match but db coulum (eg. Answer1) does not match
                                  //remove from existing filter and add from available filter
                                    selectedFilters.Remove(existingPromptFilter);
                                    selectedFilters.Add(filter);
                                    existingFilterString = string.Join(FilteringHelper.FilterSeparator, selectedFilters);
                                }
                                isFound = true; break;
                            }
                        }
                        if (!isFound)
                        {
                            selectedFilters.Remove(existingPromptFilter);
                            existingFilterString = string.Join(FilteringHelper.FilterSeparator, selectedFilters);
                            if (existingFilterString == "") existingFilterString = null;
                        }
                    }
                }
                else
                {
                    var filtersExceptPrompts = existingFilterString!.Split(FilteringHelper.FilterSeparator).Where(filter => !filter.Contains("Answer")).ToList();
                    existingFilterString = filtersExceptPrompts.Any() ? string.Join(FilteringHelper.FilterSeparator, filtersExceptPrompts) : null;
                }
            }
            return existingFilterString;
        }

        public static string? RemoveNonExistingFilterOptions(List<FilterModel> availableFilters, string existingFilterString)
        {
            var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();
            string[] filterGroups = { "LinkedToField", "AddedByAdminId", "CourseTopic", "CategoryName" };
            foreach (var filterGroup in filterGroups)
            {
                var existingFilters = existingFilterString!.Split(FilteringHelper.FilterSeparator).Where(filter => filter.Contains(filterGroup)).ToList();

                foreach (var existingFilter in existingFilters)
                {
                    bool isFound = false;
                    var splitFilter = existingFilter.Split(FilteringHelper.Separator);
                    var filterHeader = splitFilter[1];
                    var filterOptionText = splitFilter[2];
                    var availableFilterOptions = availableFilters.Where(x => x.FilterProperty == filterGroup).Select(o => o.FilterOptions).ToList();
                    foreach (var availableFilterOption in availableFilterOptions)
                    {
                        if (filterGroup == "LinkedToField")
                        {
                            if (availableFilterOption.Any(x => x.FilterValue.Contains(filterHeader)))
                            {
                                var filter = availableFilterOption.Where(x => x.FilterValue.Contains(filterHeader)).ToList().Select(x => x.FilterValue).FirstOrDefault();
                                if (!filter.Contains(filterOptionText))
                                {
                                    selectedFilters.Remove(existingFilter);
                                    selectedFilters.Add(filter);
                                    existingFilterString = string.Join(FilteringHelper.FilterSeparator, selectedFilters);
                                }
                                isFound = true; break;
                            }
                        }
                        else
                        {
                            if (availableFilterOption.Any(x => x.FilterValue.Contains(filterOptionText)))
                            {
                                isFound = true; break;
                            }
                        }
                    }

                    if (!isFound)
                    {
                        selectedFilters.Remove(existingFilter);
                        existingFilterString = string.Join(FilteringHelper.FilterSeparator, selectedFilters);
                    }
                }
            }
            if (existingFilterString == "") existingFilterString = null;
            return existingFilterString;
        }
    }
}
