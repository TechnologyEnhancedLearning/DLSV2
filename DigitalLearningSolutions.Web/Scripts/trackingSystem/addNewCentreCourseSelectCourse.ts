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
  getExistingFilterStringValue,
  IAppliedFilter,
  updateExistingFilterString,
} from '../searchSortFilterAndPaginate/filter';
import { isNullOrEmpty, sendBrowserAgnosticEvent } from '../common';

const categoryHiddenInputName = 'CategoryFilterString';
const topicHiddenInputName = 'TopicFilterString';

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

  document.getElementById('existing-filter-string')?.addEventListener('change', onFilterUpdated);
}

function setUpFilterDropdowns() {
  const filterDropdowns = Array.from(
    document.getElementsByClassName('filter-dropdown'),
  );

  filterDropdowns.forEach((filterDropdown) => {
    filterDropdown.addEventListener('change', (e) => {
      e.preventDefault();
      const newFilter = getCategoryAndTopicFilterStringAndUpdateHiddenInputs();
      updateExistingFilterString(newFilter);
    });
  });
}

function filter(searchableData: ISearchableData): void {
  let filteredSearchableElements = searchableData.searchableElements;
  const existingFilterString = getExistingFilterStringValue();

  if (!isNullOrEmpty(existingFilterString)) {
    const appliedFilters = getAppliedFilters(existingFilterString);

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
  const searchableElementsContainer = <HTMLSelectElement>document.getElementById('searchable-elements');
  if (!searchableElementsContainer) {
    return;
  }

  const selectedApplicationId = searchableElementsContainer.value;

  const defaultOption = document.getElementById('default-option');
  if (!defaultOption) {
    return;
  }
  if (searchableElements.length === 0) {
    defaultOption.innerText = 'No matching courses';
    searchableElementsContainer.value = '';
  } else {
    defaultOption.innerText = 'Select a course';
  }

  searchableElementsContainer.textContent = '';
  searchableElementsContainer.appendChild(defaultOption);
  searchableElements.forEach(
    (searchableElement) => searchableElementsContainer.appendChild(searchableElement.element),
  );

  const selectedElement = searchableElements.find(
    (x) => (<HTMLOptionElement>x.element).value === selectedApplicationId,
  );
  searchableElementsContainer.selectedIndex = selectedElement
    ? searchableElements.indexOf(selectedElement) + 1 : 0;

  // This is required to polyfill the new elements in IE
  Details();
}

function getCategoryAndTopicFilterStringAndUpdateHiddenInputs() {
  const topicFilterElement = <HTMLSelectElement>document.getElementById('CourseTopic');
  const topicFilterValue = topicFilterElement.value;
  updateExistingFilterStringHiddenInput(topicHiddenInputName, topicFilterValue);

  const categoryFilterElement = <HTMLSelectElement>document.getElementById('CategoryName');
  if (!categoryFilterElement) {
    return topicFilterValue;
  }

  const categoryFilterValue = categoryFilterElement.value;
  updateExistingFilterStringHiddenInput(categoryHiddenInputName, categoryFilterValue);

  if (isNullOrEmpty(categoryFilterValue) && isNullOrEmpty(topicFilterValue)) {
    return '';
  }

  if (isNullOrEmpty(categoryFilterValue)) {
    return topicFilterValue;
  }

  if (isNullOrEmpty(topicFilterValue)) {
    return categoryFilterValue;
  }

  return topicFilterValue + filterSeparator + categoryFilterValue;
}

function updateExistingFilterStringHiddenInput(elementName: string, newFilter: string): void {
  const existingFilterStringElements = Array.from(document.getElementsByName(elementName));

  existingFilterStringElements.forEach((existingFilterStringElement) => {
    const hiddenInput = <HTMLInputElement>existingFilterStringElement;
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
