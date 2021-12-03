import moment from 'moment';
import * as _ from 'lodash';
import { ISearchableElement } from './searchSortFilterAndPaginate';

export function setUpSort(onSortUpdated: VoidFunction): void {
  document.getElementById('select-sort-by')?.addEventListener('change', onSortUpdated);
  document.getElementById('select-sort-direction')?.addEventListener('change', onSortUpdated);
}

export function sortSearchableElements(
  searchableElements: ISearchableElement[],
): ISearchableElement[] {
  const sortBy = getSortBy();
  const sortDirection = getSortDirection();

  return _.orderBy<ISearchableElement>(
    searchableElements,
    [(searchableElement) => getSortValue(searchableElement, sortBy)],
    [(sortDirection === 'Descending') ? 'desc' : 'asc'],
  );
}

// The cases for these must match the value of the dropdown options. In most cases this will be the
// property name of the C# object to be sorted on, and should always match the value set in
// GenericSortingHelper.cs
export function getSortValue(
  searchableElement: ISearchableElement,
  sortBy: string,
): string | number | Date {
  switch (sortBy) {
    case 'SearchableName':
    case 'FullName':
    case 'Name':
      return getElementText(searchableElement, 'name').toLocaleLowerCase();
    case 'DateRegistered':
      return parseDate(getElementText(searchableElement, 'registration-date'));
    case 'StartedDate':
      return parseDate(getElementText(searchableElement, 'started-date'));
    case 'Enrolled':
      return parseDate(getElementText(searchableElement, 'enrolled-date'));
    case 'LastAccessed':
      return parseDate(getElementText(searchableElement, 'accessed-date'));
    case 'LastUpdated':
      return parseDate(getElementText(searchableElement, 'last-updated-date'));
    case 'CompleteByDate':
      return parseDate(getElementText(searchableElement, 'complete-by-date'));
    case 'Completed':
      return parseDate(getElementText(searchableElement, 'completed-date'));
    case 'HasDiagnostic,DiagnosticScore':
      return parseInt(getElementText(searchableElement, 'diagnostic-score').split('/')[0] || '-1', 10);
    case 'IsAssessed,Passes':
      return parseInt(getElementText(searchableElement, 'passed-sections').split('/')[0] || '-1', 10);
    case 'Brand':
      return getElementText(searchableElement, 'brand').toLocaleLowerCase();
    case 'Category':
      return getElementText(searchableElement, 'category').toLocaleLowerCase();
    case 'Topic':
      return getElementText(searchableElement, 'topic').toLocaleLowerCase();
    case 'DelegateCount':
      return parseInt(getElementText(searchableElement, 'delegate-count'), 10);
    case 'CoursesCount':
      return parseInt(getElementText(searchableElement, 'courses-count'), 10);
    case 'InProgressCount':
      return parseInt(getElementText(searchableElement, 'in-progress-count'), 10);
    case 'PassRate':
      return parseFloat(getElementText(searchableElement, 'pass-rate'));
    case 'CourseName':
      return getElementText(searchableElement, 'course-name').toLocaleLowerCase();
    case 'Weighting':
      return parseInt(getElementText(searchableElement, 'faq-weighting'), 10);
    default:
      return '';
  }
}

function getElementText(searchableElement: ISearchableElement, elementName: string): string {
  return searchableElement.element.querySelector(`[data-name-for-sorting="${elementName}"]`)?.textContent?.trim()
    ?? searchableElement.element.querySelector(`[name="${elementName}"]`)?.textContent?.trim()
    ?? '';
}

function parseDate(dateString: string): Date {
  const date = moment(dateString, 'DD/MM/YYYY').toDate();
  return date.toString() === 'Invalid Date' ? new Date(0) : date;
}

function getSortBy(): string {
  const element = <HTMLInputElement>document.getElementById('select-sort-by');
  return element?.value ?? 'Name';
}

function getSortDirection(): string {
  const element = <HTMLInputElement>document.getElementById('select-sort-direction');
  return element?.value ?? 'Ascending';
}
