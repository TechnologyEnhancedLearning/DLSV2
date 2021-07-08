import * as _ from 'lodash';
import { SearchableElement } from './searchSortAndPaginate';

export interface AppliedFilter {
  group: string;
  propertyName: string;
  propertyValue: string;
  unseparated: string;
}

export interface AppliedFilterTag {
  element: Element;
  filterValue: string;
}

const newlineSeparator: string = '\r\n';

export function setupFilter(onFilterUpdated: VoidFunction): void {
  const filterSubmits = Array.from(document.getElementsByClassName('filter-submit__dropdown'));
  for (let filterSubmit of filterSubmits){
    const element = <HTMLInputElement>filterSubmit;
    element.addEventListener('click', (e) => {
      e.preventDefault();
      appendNewFilterToFilterBy(element);
    });
  }

  document.getElementById('clear-filters')?.addEventListener('click', (e) => {
    e.preventDefault();
    clearFilters();
  })

  document.getElementById('filter-by')?.addEventListener('change', onFilterUpdated);
}

export function filterSearchableElements(
  searchableElements: SearchableElement[], possibleFilters: AppliedFilterTag[]
): SearchableElement[] {
  const filterBy = getFilterBy();
  const listOfFilters = filterBy.split(newlineSeparator);
  const appliedFilters = listOfFilters.map((filter) => {
    const splitFilter = filter.split('|');
    return <AppliedFilter>{
      group : splitFilter[0],
      propertyName : splitFilter[1],
      propertyValue : splitFilter[2],
      unseparated : filter
    }
  });

  const filterGroups = _.groupBy(appliedFilters, (filter) => filter.group);

  _.forEach(filterGroups, (appliedFiltersInGroup) => {
    const itemsToFilter = searchableElements;
    const setOfFilteredLists = _.map(appliedFiltersInGroup, (af) => filterElements(itemsToFilter, af));
    const flattenedElements = _.flatten(setOfFilteredLists);
    searchableElements = _.uniq(flattenedElements);
  })

  if (filterBy){
    updateAppliedFilters(filterBy, possibleFilters);
    showAppliedFilters();
  }

  return searchableElements;
}

function filterElements(searchableElements: SearchableElement[], appliedFilter: AppliedFilter): SearchableElement[]{
  return _.filter<SearchableElement>(
    searchableElements,
    function(o) {
      return doesElementHaveFilterValue(o, appliedFilter.unseparated);
      // const elementFilterValue = getElementFilterValue(o, propertyName);
      // return elementFilterValue === filterValue;
    });
}

function appendNewFilterToFilterBy(filterSubmit: HTMLInputElement): void {
  // grab new filter string from filter dropdown
  const filterId = filterSubmit.id.replace('-submit', '');
  const filterElement = <HTMLSelectElement>document.getElementById(filterId);
  const filterValue = filterElement.value;
  // update filter-by hidden with string
  const filterBy = getFilterBy();
  if (!filterBy.includes(filterValue)){
    let newFilter;
    if (filterBy){
      newFilter = filterBy + newlineSeparator + filterValue;
    } else {
      newFilter = filterValue;
    }
    updateAllFilterByElementsByName(newFilter);
    updateFilterByById(newFilter);

  }
  filterElement.selectedIndex = 0;

  return;
}

function clearFilters(){
  updateAllFilterByElementsByName('');
  updateFilterByById('');
  clearAppliedFilters();
  hideAppliedFilters();
}

function doesElementHaveFilterValue(searchableElement: SearchableElement, filter: string): boolean{
  const filterElement = searchableElement.element.querySelector(`[data-filter-value="${filter}"]`);
  const filterValue = filterElement?.getAttribute('data-filter-value')?.trim() ?? '';
  return filterValue === filter;
}

export function getFilterBy(): string {
  const element = <HTMLInputElement>document.getElementById('filter-by');
  return element.value;
}

function updateFilterByById(newFilter: string): void {
  const element = <HTMLInputElement>document.getElementById('filter-by');
  element.value = newFilter;
  element.dispatchEvent(new Event('change'));
}

function updateAllFilterByElementsByName(newFilter: string): void {
  const filterByElements = Array.from(document.getElementsByName('filterBy'));
  for (let filterElement of filterByElements){
    const element = <HTMLInputElement>filterElement;
    element.value = newFilter;
  }
}

function showAppliedFilters(): void{
  const element = document.getElementById('applied-filters');
  if (element){
    element.hidden = false;
  }
}

function hideAppliedFilters(): void{
  const element = document.getElementById('applied-filters');
  if (element){
    element.hidden = true;
  }
}

function clearAppliedFilters(){
  const appliedFilterContainer = document.getElementById('applied-filter-container');
  if (!appliedFilterContainer) {
    return;
  }

  appliedFilterContainer.textContent = '';
}

function updateAppliedFilters(filterBy: string, possibleFilters: AppliedFilterTag[]) {
  const appliedFilterContainer = document.getElementById('applied-filter-container');
  if (!appliedFilterContainer) {
    return;
  }
  const listOfFilters = filterBy.split('\r\n');

  appliedFilterContainer.textContent = '';
  listOfFilters.forEach(
    (filter) => appliedFilterContainer.appendChild(getMatchingFilterTag(possibleFilters, filter)),
  );
}

function getMatchingFilterTag(possibleFilters: AppliedFilterTag[], filter: string): Element{
  const appliedFilterTag = _.find(possibleFilters, (pf) => pf.filterValue === filter);
  return appliedFilterTag!.element;
}
