import {
  ISearchableData,
  ISearchableElement,
  SearchSortFilterAndPaginate
} from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import {
  filterSeparator,
  getAppliedFilters,
  getFilterByValue,
  IAppliedFilter,
  updateFilterBy
} from "../searchSortFilterAndPaginate/filter";
import * as _ from "lodash";
import Details from "nhsuk-frontend/packages/components/details/details";

// eslint-disable-next-line no-new
SearchSortFilterAndPaginate.getSearchableElements(`TrackingSystem/CourseSetup/AddCourse/SelectCourseAllCourses`, ['title'])
  .then((searchableData) => {
    if (searchableData === undefined) {
      return;
    }

    setUpFilter(() => filter(searchableData));

    filter(searchableData);
  });

function filter(searchableData: ISearchableData): void {
  let filteredSearchableElements = searchableData.searchableElements;
  const filterBy = getFilterByValue();

  if (typeof filterBy != undefined && filterBy) {

    const appliedFilters = getAppliedFilters(filterBy);

    const filterGroups = _.groupBy(appliedFilters, (filter) => filter.group);

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

function displaySearchableElements(searchableElements: ISearchableElement[]): void {
  const searchableElementsContainer = document.getElementById('searchable-elements');
  if (!searchableElementsContainer) {
    return;
  }

  const defaultOption = document.getElementById('default-option');
  if (!defaultOption) {
    return;
  }
  defaultOption.innerText = searchableElements.length === 0 ? "No matching courses" : "Select a course";

  searchableElementsContainer.textContent = '';
  searchableElementsContainer.appendChild(defaultOption);
  searchableElements.forEach(
    (searchableElement) => searchableElementsContainer.appendChild(searchableElement.element),
  );

  console.log(searchableElementsContainer);

  // This is required to polyfill the new elements in IE
  Details();
}

function setUpFilter(onFilterUpdated: VoidFunction): void {

  setUpFilterSubmitButtons();

  document.getElementById('filter-by')?.addEventListener('change', onFilterUpdated);
}

function setUpFilterSubmitButtons() {
  const filterSubmits = Array.from(
    document.getElementsByClassName('filter-submit'),
  );
  filterSubmits.forEach((filterSubmit) => {
    const element = <HTMLInputElement>filterSubmit;
    element.addEventListener('click', (e) => {
      e.preventDefault();
      const newFilter = getCategoryAndTopicFilterBy();

      updateFilterBy(newFilter!);
    });
  });
}

function getCategoryAndTopicFilterBy() {
  const categoryFilterElement = <HTMLSelectElement>document.getElementById('CategoryName');
  const categoryFilterValue = categoryFilterElement.value;

  const topicFilterElement = <HTMLSelectElement>document.getElementById('CourseTopic');
  const topicFilterValue = topicFilterElement.value;

  if (isNullOrEmpty(categoryFilterValue) && isNullOrEmpty(topicFilterValue)) {
    return null;
  }

  if (isNullOrEmpty(categoryFilterValue)) {
    return topicFilterValue;
  }

  if (isNullOrEmpty(topicFilterValue)) {
    return categoryFilterValue;
  }

  return topicFilterValue + filterSeparator + categoryFilterValue;
}

function filterElements(
  searchableElements: ISearchableElement[],
  appliedFilter: IAppliedFilter,
): ISearchableElement[] {
  return searchableElements.filter(
    (element) => doesElementMatchFilterValue(element, appliedFilter.filterValue),
  );
}

function doesElementMatchFilterValue(
  searchableElement: ISearchableElement,
  filter: string,
): boolean {
  const categoryFilterValue = searchableElement.element.getAttribute('category-filter-value')?.trim() ?? '';
  const topicFilterValue = searchableElement.element.getAttribute('topic-filter-value')?.trim() ?? '';
  return categoryFilterValue === filter || topicFilterValue === filter;
}

function isNullOrEmpty(
  filterValue: string,
): boolean {
  return typeof filterValue == undefined || !filterValue
}
