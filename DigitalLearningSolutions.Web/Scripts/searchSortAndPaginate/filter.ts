import * as _ from 'lodash';
import {SearchableElement} from './searchSortAndPaginate';

export interface AppliedFilter {
  group: string;
  propertyName: string;
  propertyValue: string;
  filterValue: string;
}

export interface AppliedFilterTag {
  element: Element;
  filterValue: string;
}

export const separator: string ='|';
export const filterSeparator: string = '╡';

export function setupFilter(onFilterUpdated: VoidFunction): void {
  setUpFilterSubmitButtons();
  setUpClearFiltersButton();
  setUpFilterSelectorDropdown();
  hideAllFilterDropdowns();
  document.getElementById('filter-by')?.addEventListener('change', onFilterUpdated);
}

export function filterSearchableElements(
  searchableElements: SearchableElement[], possibleFilters: AppliedFilterTag[]
): SearchableElement[] {
  const filterBy = getFilterBy();
  const appliedFilters = getAppliedFilters(filterBy);

  const filterGroups = _.groupBy(appliedFilters, (filter) => filter.group);

  _.forEach(filterGroups, (appliedFiltersInGroup) => {
    const itemsToFilter = searchableElements;
    const setOfFilteredLists = _.map(appliedFiltersInGroup, (af) => filterElements(itemsToFilter, af));
    const flattenedElements = _.flatten(setOfFilteredLists);
    searchableElements = _.uniq(flattenedElements);
  })

  if (filterBy) {
    updateAppliedFilters(filterBy, possibleFilters);
    showAppliedFilters();
  }

  return searchableElements;
}

function setUpFilterSubmitButtons() {
  const filterSubmits = Array.from(document.getElementsByClassName('filter-submit__dropdown'));
  for (let filterSubmit of filterSubmits) {
    const element = <HTMLInputElement>filterSubmit;
    element.addEventListener('click', (e) => {
      e.preventDefault();
      appendNewFilterToFilterBy(element);
      hideAllFilterDropdowns();
      resetFilterSelectorDropdown();
    });
  }
}

function setUpClearFiltersButton() {
  document.getElementById('clear-filters')?.addEventListener('click', (e) => {
    e.preventDefault();
    clearFilters();
  })
}

function setUpFilterSelectorDropdown() {
  document.getElementById('filter-selector')?.addEventListener('change', (e) => {
    showSelectedFilterDropdown();
  })
}

function getAppliedFilters(filterBy: string): AppliedFilter[] {
  return filterBy.split(filterSeparator).map((filter) => {
    return newAppliedFilterFromFilter(filter);
  });
}

function newAppliedFilterFromFilter(filter: string): AppliedFilter {
  const splitFilter = filter.split(separator);
  return <AppliedFilter>{
    group: splitFilter[0],
    propertyName: splitFilter[1],
    propertyValue: splitFilter[2],
    filterValue: filter
  }
}

function filterElements(searchableElements: SearchableElement[], appliedFilter: AppliedFilter): SearchableElement[] {
  return _.filter<SearchableElement>(
    searchableElements,
    function (o) {
      return doesElementMatchFilterValue(o, appliedFilter.filterValue);
    });
}

function appendNewFilterToFilterBy(filterSubmit: HTMLInputElement): void {
  const filterValue = getSelectedFilterFromDropdownAndResetDropdown(filterSubmit)
  addNewFilterToFilterBy(filterValue);
}

function getSelectedFilterFromDropdownAndResetDropdown(filterSubmit: HTMLInputElement): string {
  const filterId = filterSubmit.id.replace('-submit', '');
  const filterElement = <HTMLSelectElement>document.getElementById(filterId);
  const filterValue = filterElement.value;
  filterElement.selectedIndex = 0;
  return filterValue;
}

function addNewFilterToFilterBy(filterValue: string): void {
  const filterBy = getFilterBy();
  if (!filterBy.includes(filterValue)) {
    let newFilter;
    if (filterBy) {
      newFilter = filterBy + filterSeparator + filterValue;
    } else {
      newFilter = filterValue;
    }
    updateAllFilterByElementsByName(newFilter);
    updateFilterByById(newFilter);
  }
}

function clearFilters(): void {
  updateAllFilterByElementsByName('');
  updateFilterByById('');
  clearAppliedFilters();
  hideAppliedFilters();
}

function hideAllFilterDropdowns(): void {
  const allDropdowns = Array.from(document.getElementsByClassName('filter-dropdown-container'));
  for (let dropdown of allDropdowns) {
    const dropdownElement = <HTMLElement>dropdown;
    dropdownElement.hidden = true;
  }
}

function showSelectedFilterDropdown(): void {
  hideAllFilterDropdowns();

  const selector = getFilterSelectorDropdown();
  const selectedDropdown = selector.value;
  const selectedDropdownElement = <HTMLElement>document.getElementById(selectedDropdown);
  selectedDropdownElement.hidden = false;
}

function resetFilterSelectorDropdown(): void {
  const selector = getFilterSelectorDropdown();
  selector.selectedIndex = 0;
}

function doesElementMatchFilterValue(searchableElement: SearchableElement, filter: string): boolean {
  const filterElement = searchableElement.element.querySelector(`[data-filter-value="${filter}"]`);
  const filterValue = filterElement?.getAttribute('data-filter-value')?.trim() ?? '';
  return filterValue === filter;
}

export function getFilterBy(): string {
  return getFilterByElement().value;
}

function updateFilterByById(newFilter: string): void {
  const element = getFilterByElement();
  element.value = newFilter;
  element.dispatchEvent(new Event('change'));
}

function getFilterByElement(): HTMLInputElement {
  return <HTMLInputElement>document.getElementById('filter-by');
}

function updateAllFilterByElementsByName(newFilter: string): void {
  const filterByElements = Array.from(document.getElementsByName('filterBy'));
  for (let filterElement of filterByElements) {
    const element = <HTMLInputElement>filterElement;
    element.value = newFilter;
  }
}

function showAppliedFilters(): void {
  const element = getAppliedFiltersElement();
  element.hidden = false;
}

function hideAppliedFilters(): void {
  const element = getAppliedFiltersElement();
  element.hidden = true;
}

function clearAppliedFilters() {
  const appliedFilterContainer = getAppliedFilterContainer();
  appliedFilterContainer.textContent = '';
}

function updateAppliedFilters(filterBy: string, possibleFilters: AppliedFilterTag[]) {
  const appliedFilterContainer = getAppliedFilterContainer();
  const listOfFilters = filterBy.split(filterSeparator);
  appliedFilterContainer.textContent = '';
  listOfFilters.forEach(
    (filter) => appliedFilterContainer.appendChild(getMatchingFilterTag(possibleFilters, filter)),
  );
}

function getMatchingFilterTag(possibleFilters: AppliedFilterTag[], filter: string): Element {
  return _.find(possibleFilters, (pf) => pf.filterValue === filter)!.element;
}

function getAppliedFilterContainer(): Element {
  return <HTMLElement>document.getElementById('applied-filter-container');
}

function getAppliedFiltersElement(): HTMLElement {
  return <HTMLElement>document.getElementById('applied-filters');
}

function getFilterSelectorDropdown(): HTMLSelectElement {
  return <HTMLSelectElement>document.getElementById('filter-selector');
}

