import * as _ from 'lodash';
import { SearchableElement } from './searchSortAndPaginate';

export function setupFilter(onFilterUpdated: VoidFunction): void {
  const filterSubmits = Array.from(document.getElementsByClassName('filter-submit__dropdown'));
  for (let filterSubmit of filterSubmits){
    const element = <HTMLInputElement>filterSubmit;
    element.addEventListener('click', (e) => {
      e.preventDefault();
      appendNewFilterToFilterBy(element);
    });
  }
  document.getElementById('filter-by')?.addEventListener('change', onFilterUpdated);
}

export function filterSearchableElements(
  searchableElements: SearchableElement[],
): SearchableElement[] {
  const filterBy = getFilterBy();
  const setOfSelectedFilters = filterBy.split('\r\n');

  for (let filter of setOfSelectedFilters){
    if (filter){
      searchableElements = _.filter<SearchableElement>(
        searchableElements,
        function(o) {
          return doesElementHaveFilterValue(o, filter);
          // const elementFilterValue = getElementFilterValue(o, propertyName);
          // return elementFilterValue === filterValue;
        }
      );
      showAppliedFilters();
    }
  }
  return searchableElements;
}

function appendNewFilterToFilterBy(filterSubmit: HTMLInputElement): void {
  // grab new filter string from filter dropdown
  const submitId = filterSubmit.id;
  const filterId = submitId.replace('-submit', '');
  const filterElement = <HTMLSelectElement>document.getElementById(filterId);
  const filterValue = filterElement.value;
  // update filter-by hidden with string
  const filterBy = getFilterBy();
  if (!filterBy.includes(filterValue)){
    let newFilter;
    if (filterBy){
      newFilter = filterBy + '\r\n' + filterValue;
    } else {
      newFilter = filterValue;
    }
    updateAllFilterByElementsByName(newFilter);
    updateFilterByById(newFilter);
  }
  return;
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


