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
    const splitFilter = filter.split('|');
    const propertyName = splitFilter[0];
    const filterValue = splitFilter[1];

    searchableElements = _.filter<SearchableElement>(
      searchableElements,
      function(o) { return getElementText(o, propertyName) === filterValue }
    );
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
    updateFilterBy(newFilter);
  }
  return;
}

function getElementText(searchableElement: SearchableElement, elementName: string): string {
  return searchableElement.element.querySelector(`[name="${elementName}"]`)?.textContent?.trim() ?? '';
}

function getFilterBy(): string {
  const element = <HTMLInputElement>document.getElementById('filter-by');
  return element.value;
}

function updateFilterBy(newFilter: string): void {
  const element = <HTMLInputElement>document.getElementById('filter-by');
  element.value = newFilter;
}


