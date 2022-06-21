import * as _ from 'lodash';
import Cookies from 'js-cookie';
import { ISearchableElement } from './searchSortFilterAndPaginate';
import { sendBrowserAgnosticEvent } from '../common';

export interface IAppliedFilter {
  group: string;
  propertyName: string;
  propertyValue: string;
  filterValue: string;
}

export interface IAppliedFilterTag {
  element: Element;
  filterValue: string;
}

export const separator = '|';
export const filterSeparator = '╡';
const cookieMaxLifeInDays = 30;
let cookieName: string;

export function setUpFilter(onFilterUpdated: VoidFunction, filterCookieName: string): void {
  cookieName = filterCookieName;

  setUpFilterSubmitButtons();
  setUpClearFiltersButton();
  setUpFilterSelectorDropdown();
  hideAllFilterDropdowns();
  document.getElementById('existing-filter-string')?.addEventListener('change', onFilterUpdated);
}

export function filterSearchableElements(
  searchableElements: ISearchableElement[],
  possibleFilters: IAppliedFilterTag[],
): ISearchableElement[] {
  let filteredSearchableElements = searchableElements;
  const existingFilterString = getExistingFilterStringValue();
  const appliedFilters = getAppliedFilters(existingFilterString);

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

  if (existingFilterString) {
    updateAppliedFilters(existingFilterString, possibleFilters);
    showAppliedFilters();
  }

  return filteredSearchableElements;
}

function setUpFilterSubmitButtons() {
  const filterSubmits = Array.from(
    document.getElementsByClassName('filter-submit'),
  );
  filterSubmits.forEach((filterSubmit) => {
    const element = <HTMLInputElement>filterSubmit;
    element.addEventListener('click', (e) => {
      e.preventDefault();
      const newFilter = appendNewFilterToExistingFilterString(element);
      if (newFilter) {
        hideAllFilterDropdowns();
        resetFilterSelectorDropdown();
      }
    });
  });
}

function setUpClearFiltersButton() {
  document.getElementById('clear-filters')?.addEventListener('click', (e) => {
    e.preventDefault();
    clearFilters();
  });
}

function setUpFilterSelectorDropdown() {
  document.getElementById('filter-selector')?.addEventListener('change', () => {
    showSelectedFilterDropdown();
  });
}

export function getAppliedFilters(existingFilterString: string): IAppliedFilter[] {
  return existingFilterString.split(filterSeparator)
    .map((filter) => newAppliedFilterFromFilter(filter));
}

function newAppliedFilterFromFilter(filter: string): IAppliedFilter {
  const splitFilter = filter.split(separator);
  return <IAppliedFilter>{
    group: splitFilter[0],
    propertyName: splitFilter[1],
    propertyValue: splitFilter[2],
    filterValue: filter,
  };
}

function filterElements(
  searchableElements: ISearchableElement[],
  appliedFilter: IAppliedFilter,
): ISearchableElement[] {
  return searchableElements.filter(
    (element) => doesElementMatchFilterValue(element, appliedFilter.filterValue),
  );
}

function appendNewFilterToExistingFilterString(filterSubmit: HTMLInputElement): string {
  const newFilterToAdd = getSelectedFilterFromDropdownAndResetDropdown(filterSubmit);
  addNewFilterValueToExistingFilterString(newFilterToAdd);
  return newFilterToAdd;
}

function getSelectedFilterFromDropdownAndResetDropdown(
  filterSubmit: HTMLInputElement,
): string {
  const filterId = filterSubmit.id.replace('-submit', '');
  const filterElement = <HTMLSelectElement>document.getElementById(filterId);
  const filterValue = filterElement.value;
  filterElement.selectedIndex = 0;
  return filterValue;
}

function addNewFilterValueToExistingFilterString(newFilterValue: string): void {
  if (!newFilterValue) return;

  const existingFilterString = getExistingFilterStringValue();
  if (!existingFilterString.split(filterSeparator).includes(newFilterValue)) {
    const updatedExistingFilterString = existingFilterString
      ? existingFilterString + filterSeparator + newFilterValue
      : newFilterValue;
    updateAllExistingFilterStringHiddenInputs(updatedExistingFilterString);
    updateExistingFilterString(updatedExistingFilterString);
    updateFilterCookieValue(updatedExistingFilterString);
  }
}

function clearFilters(): void {
  updateAllExistingFilterStringHiddenInputs('');
  updateExistingFilterString('');
  updateFilterCookieValue('CLEAR');
  clearAppliedFilters();
  hideAppliedFilters();
}

function hideAllFilterDropdowns(): void {
  const allDropdowns = <HTMLElement[]>Array.from(
    document.getElementsByClassName('filter-dropdown-container'),
  );
  allDropdowns.forEach((dropdown) => {
    const dropdownElement = dropdown;
    dropdownElement.hidden = true;
    dropdownElement.style.display = 'none';
    dropdownElement.setAttribute('aria-hidden', 'true');
  });
}

function showSelectedFilterDropdown(): void {
  hideAllFilterDropdowns();

  const selector = getFilterSelectorDropdown();
  const selectedDropdown = selector.value;
  const selectedDropdownElement = <HTMLElement>document.getElementById(selectedDropdown);
  selectedDropdownElement.hidden = false;
  selectedDropdownElement.style.display = 'block';
  selectedDropdownElement.setAttribute('aria-hidden', 'false');
}

function resetFilterSelectorDropdown(): void {
  const selector = getFilterSelectorDropdown();
  selector.selectedIndex = 0;
}

function doesElementMatchFilterValue(
  searchableElement: ISearchableElement,
  filter: string,
): boolean {
  const filterElement = searchableElement.element.querySelector(
    // Escape " in `filter` because `[attr=""value""]` is not a valid selector.
    // The `g` flag on the regex means "greedy", i.e. match multiple times (with every match being replaced).
    `[data-filter-value="${filter.replace(/"/g, '\\"')}"]`,
  );
  const filterValue = filterElement?.getAttribute('data-filter-value')?.trim() ?? '';
  return filterValue === filter;
}

export function getExistingFilterStringValue(): string {
  return getExistingFilterStringElement().value;
}

export function updateExistingFilterString(newFilter: string): void {
  const element = getExistingFilterStringElement();
  element.value = newFilter;
  sendBrowserAgnosticEvent(element, 'change');
}

function updateFilterCookieValue(newFilter: string): void {
  Cookies.set(cookieName, newFilter, { expires: cookieMaxLifeInDays });
}

function getExistingFilterStringElement(): HTMLInputElement {
  return <HTMLInputElement>document.getElementById('existing-filter-string');
}

function updateAllExistingFilterStringHiddenInputs(newFilter: string): void {
  const existingFilterStringElements = Array.from(document.getElementsByName('existingFilterString'));
  existingFilterStringElements.forEach((filterElement) => {
    const element = <HTMLInputElement>filterElement;
    element.value = newFilter;
  });
}

function showAppliedFilters(): void {
  const element = getAppliedFiltersElement();
  element.hidden = false;
  element.setAttribute('aria-hidden', 'false');
}

function hideAppliedFilters(): void {
  const element = getAppliedFiltersElement();
  element.hidden = true;
  element.setAttribute('aria-hidden', 'true');
}

function clearAppliedFilters() {
  const appliedFilterContainer = getAppliedFilterContainer();
  appliedFilterContainer.textContent = '';
}

function updateAppliedFilters(existingFilterString: string, possibleFilters: IAppliedFilterTag[]) {
  const appliedFilterContainer = getAppliedFilterContainer();
  const listOfFilters = existingFilterString.split(filterSeparator);
  appliedFilterContainer.textContent = '';

  listOfFilters.forEach(
    (filter) => {
      appliedFilterContainer.appendChild(getMatchingFilterTag(possibleFilters, filter));
      appliedFilterContainer.appendChild(document.createTextNode(' '));
    },
  );
}

function getMatchingFilterTag(possibleFilters: IAppliedFilterTag[], filter: string): Element {
  const appliedFilterTag = <IAppliedFilterTag>possibleFilters.find(
    (pf) => pf.filterValue === filter,
  );
  return appliedFilterTag.element;
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
