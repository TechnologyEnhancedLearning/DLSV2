import moment from 'moment';
import * as _ from 'lodash';
import { SearchableElement } from './searchSortAndPaginate';

export function setupSort(onSortUpdated: VoidFunction): void {
  document.getElementById('select-sort-by')?.addEventListener('change', onSortUpdated);
  document.getElementById('select-sort-direction')?.addEventListener('change', onSortUpdated);
}

export function sortSearchableElements(
  searchableElements: SearchableElement[],
): SearchableElement[] {
  const sortBy = getSortBy();
  const sortDirection = getSortDirection();

  return _.orderBy<SearchableElement>(
    searchableElements,
    [(searchableElement) => getSortValue(searchableElement, sortBy)],
    [(sortDirection === 'Descending') ? 'desc' : 'asc'],
  );
}

// The cases for these must match the value of the dropdown options. In most cases this will be the
// property name of the C# object to be sorted on, and should always match the value set in
// GenericSortingHelper.cs
export function getSortValue(
  searchableElement: SearchableElement,
  sortBy: string,
): string | number | Date {
  switch (sortBy) {
    case 'Name':
      return getElementText(searchableElement, 'name').toLocaleLowerCase();
    case 'StartedDate':
      return parseDate(getElementText(searchableElement, 'started-date'));
    case 'LastAccessed':
      return parseDate(getElementText(searchableElement, 'accessed-date'));
    case 'CompleteByDate':
      return parseDate(getElementText(searchableElement, 'complete-by-date'));
    case 'Completed':
      return parseDate(getElementText(searchableElement, 'completed-date'));
    case 'HasDiagnostic,DiagnosticsScore':
      return parseInt(getElementText(searchableElement, 'diagnostic-score').split('/')[0] || '-1', 10);
    case 'IsAssessed,Passes':
      return parseInt(getElementText(searchableElement, 'passed-sections').split('/')[0] || '-1', 10);
    case 'Brand':
      return getElementText(searchableElement, 'brand').toLocaleLowerCase();
    case 'Category':
      return getElementText(searchableElement, 'category').toLocaleLowerCase();
    case 'Topic':
      return getElementText(searchableElement, 'topic').toLocaleLowerCase();
    default:
      return '';
  }
}

function getElementText(searchableElement: SearchableElement, elementName: string): string {
  return searchableElement.element.querySelector(`[name="${elementName}"]`)?.textContent?.trim() ?? '';
}

function parseDate(dateString: string): Date {
  const date = moment(dateString, 'DD/MM/YYYY').toDate();
  return date.toString() === 'Invalid Date' ? new Date(0) : date;
}

function getSortBy(): string {
  const element = <HTMLInputElement>document.getElementById('select-sort-by');
  return element.value;
}

function getSortDirection(): string {
  const element = <HTMLInputElement>document.getElementById('select-sort-direction');
  return element.value;
}
