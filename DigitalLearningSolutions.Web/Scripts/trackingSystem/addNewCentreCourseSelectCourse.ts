import * as _ from 'lodash';
import Details from 'nhsuk-frontend/packages/components/details/details';
import {
  ISearchableData,
  ISearchableElement,
  SearchSortFilterAndPaginate,
} from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import {
  filterSeparator,
  getAppliedFilters,
  getFilterByValue,
  IAppliedFilter,
  updateFilterBy,
} from '../searchSortFilterAndPaginate/filter';
import { sendBrowserAgnosticEvent } from '../common';

const categoryHiddenInputName = 'CategoryFilterBy';
const topicHiddenInputName = 'TopicFilterBy';

// eslint-disable-next-line no-new
SearchSortFilterAndPaginate.getSearchableElements('TrackingSystem/CourseSetup/AddCourse/SelectCourseAllCourses', ['title'])
  .then((searchableData) => {
    if (searchableData === undefined) {
      return;
    }

    setUpFilter(() => filter(searchableData));

    filter(searchableData);
  });

function setUpFilter(onFilterUpdated: VoidFunction): void {
  setUpFilterDropdowns();

  document.getElementById('filter-by')?.addEventListener('change', onFilterUpdated);
}

function setUpFilterDropdowns() {
  const filterDropdowns = Array.from(
    document.getElementsByClassName('filter-dropdown'),
  );

  filterDropdowns.forEach((filterDropdown) => {
    filterDropdown.addEventListener('change', (e) => {
      e.preventDefault();
      const newFilter = getCategoryAndTopicFilterByAndUpdateHiddenInputs();

      if (newFilter != null) {
        updateFilterBy(newFilter);
      }
    });
  });
}

function filter(searchableData: ISearchableData): void {
  let filteredSearchableElements = searchableData.searchableElements;
  const filterBy = getFilterByValue();

  if (typeof filterBy !== undefined && filterBy) {
    const appliedFilters = getAppliedFilters(filterBy);

    const filterGroups = _.groupBy(appliedFilters, (appliedFilter) => appliedFilter.group);

    _.forEach(filterGroups, (appliedFiltersInGroup) => {
      const itemsToFilter = filteredSearchableElements;
      const setOfFilteredLists = _.map(
        appliedFiltersInGroup,
        (af) => filterElements(itemsToFilter, af),
      );
      const flattenedElements = _.flatten(setOfFilteredLists);
      filteredSearchableElements = _.uniq(flattenedElements);
    });
  }

  displaySearchableElements(filteredSearchableElements);
}

function filterElements(
  searchableElements: ISearchableElement[],
  appliedFilter: IAppliedFilter,
): ISearchableElement[] {
  return searchableElements.filter(
    (element) => doesElementMatchFilterValue(element, appliedFilter.filterValue),
  );
}

function displaySearchableElements(searchableElements: ISearchableElement[]): void {
  const searchableElementsContainer = document.getElementById('searchable-elements');
  if (!searchableElementsContainer) {
    return;
  }

  const defaultOption = document.getElementById('default-option');
  if (!defaultOption) {
    return;
  }
  defaultOption.innerText = searchableElements.length === 0 ? 'No matching courses' : 'Select a course';

  searchableElementsContainer.textContent = '';
  searchableElementsContainer.appendChild(defaultOption);
  searchableElements.forEach(
    (searchableElement) => searchableElementsContainer.appendChild(searchableElement.element),
  );

  // This is required to polyfill the new elements in IE
  Details();
}

function getCategoryAndTopicFilterByAndUpdateHiddenInputs() {
  const categoryFilterElement = <HTMLSelectElement>document.getElementById('CategoryName');
  const categoryFilterValue = categoryFilterElement.value;

  const topicFilterElement = <HTMLSelectElement>document.getElementById('CourseTopic');
  const topicFilterValue = topicFilterElement.value;

  if (isNullOrEmpty(categoryFilterValue) && isNullOrEmpty(topicFilterValue)) {
    return null;
  }

  if (isNullOrEmpty(categoryFilterValue)) {
    updateFilterByHiddenInput(topicHiddenInputName, topicFilterValue);
    return topicFilterValue;
  }

  if (isNullOrEmpty(topicFilterValue)) {
    updateFilterByHiddenInput(categoryHiddenInputName, categoryFilterValue);
    return categoryFilterValue;
  }

  updateFilterByHiddenInput(categoryHiddenInputName, categoryFilterValue);
  updateFilterByHiddenInput(topicHiddenInputName, topicFilterValue);

  return topicFilterValue + filterSeparator + categoryFilterValue;
}

function updateFilterByHiddenInput(elementName: string, newFilter: string): void {
  const filterByElements = Array.from(document.getElementsByName(elementName));

  filterByElements.forEach((filterByElement) => {
    const hiddenInput = <HTMLInputElement>filterByElement;
    hiddenInput.value = newFilter;
    sendBrowserAgnosticEvent(hiddenInput, 'change');
  });
}

function doesElementMatchFilterValue(
  searchableElement: ISearchableElement,
  filterValue: string,
): boolean {
  const categoryFilterValue = searchableElement.element.getAttribute('category-filter-value')?.trim() ?? '';
  const topicFilterValue = searchableElement.element.getAttribute('topic-filter-value')?.trim() ?? '';
  return categoryFilterValue === filterValue || topicFilterValue === filterValue;
}

function isNullOrEmpty(
  filterValue: string,
): boolean {
  return typeof filterValue === undefined || !filterValue;
}
