import { parse as parseDate, isValid as isValidDate } from 'date-fns';
import * as _ from 'lodash';
import { ISearchableElement } from './searchSortFilterAndPaginate';

export function setUpSort(onSortUpdated: VoidFunction): void {
  document.getElementById('select-sort-by')?.addEventListener('change', onSortUpdated);
  document.getElementById('select-sort-direction')?.addEventListener('change', onSortUpdated);
}

export function sortSearchableElements(
  searchableElements: ISearchableElement[],
): ISearchableElement[] {
  const sortByValue = getSortBy();
  const fieldsToSortBy = document.querySelector('[data-sort-by-multiple]') ? sortByValue.split(',') : [sortByValue];
  const sortDirection = getSortDirection();

  return _.orderBy<ISearchableElement>(
    searchableElements,
    fieldsToSortBy
      .map((sortBy: string) => (searchableElement) => getSortValue(searchableElement, sortBy)),
    fieldsToSortBy.map(() => ((sortDirection === 'Descending') ? 'desc' : 'asc')),
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
    case 'FullNameForSearchingSorting':
    case 'SearchableName':
    case 'FullName':
    case 'Name':
      return getElementText(searchableElement, 'name').toLocaleLowerCase();
    case 'DateRegistered':
      return parseDateAndTime(getElementText(searchableElement, 'registration-date'));
    case 'StartedDate':
      return parseDateAndTime(getElementText(searchableElement, 'started-date'));
    case 'Enrolled':
      return parseDateAndTime(getElementText(searchableElement, 'enrolled-date'));
    case 'LastAccessed':
      return parseDateAndTime(getElementText(searchableElement, 'accessed-date'));
    case 'LastUpdated':
      return parseDateAndTime(getElementText(searchableElement, 'last-updated-date'));
    case 'CompleteByDate':
      return parseDateAndTime(getElementText(searchableElement, 'complete-by-date'));
    case 'Completed':
      return parseDateAndTime(getElementText(searchableElement, 'completed-date'));
    case 'CreatedDate':
      return parseDateAndTime(getElementText(searchableElement, 'created-date'));
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
    case 'CompletedCount':
      return parseInt(getElementText(searchableElement, 'completed-count'), 10);
    case 'PassRate':
      return parseFloat(getElementText(searchableElement, 'pass-rate'));
    case 'CourseName':
    case 'ApplicationName':
      return getElementText(searchableElement, 'course-name').toLocaleLowerCase();
    case 'Weighting':
      return parseInt(getElementText(searchableElement, 'faq-weighting'), 10);
    case 'FaqId':
      return parseInt(getElementText(searchableElement, 'faq-id'), 10);
    case 'CandidateNumber':
      return getElementText(searchableElement, 'delegate-id').toLocaleLowerCase();
    case 'When':
      return parseDateAndTime(getElementText(searchableElement, 'when'));
    case 'LearningTime':
    case 'TotalMins':
      return parseNonNegativeIntOrNotApplicable(getElementText(searchableElement, 'learning-time'));
    case 'AssessmentScore':
      return parseNonNegativeIntOrNotApplicable(getElementText(searchableElement, 'assessment-score'));
    case 'PopularityRating':
      return parseFloat(getElementText(searchableElement, 'popularity-score'));
    default:
      return '';
  }
}

function getElementText(searchableElement: ISearchableElement, elementName: string): string {
  return searchableElement.element.querySelector(`[data-name-for-sorting="${elementName}"]`)?.textContent?.trim()
    ?? searchableElement.element.querySelector(`[name="${elementName}"]`)?.textContent?.trim()
    ?? '';
}

function parseDateAndTime(dateString: string): Date {
  const possibleFormats = [
    'dd/MM/yyyy HH:mm',
    'dd/MM/yyyy',
  ];

  for (let i = 0; i < possibleFormats.length; i += 1) {
    const dateAndTime = parseDate(dateString, possibleFormats[i], new Date());

    if (isValidDate(dateAndTime)) {
      return dateAndTime;
    }
  }

  return new Date(0);
}

function parseNonNegativeIntOrNotApplicable(value: string): number {
  return value === 'N/A' ? -1 : parseInt(value, 10);
}

export function getSortBy(): string {
  const element = <HTMLInputElement>document.getElementById('select-sort-by');
  return element?.value ?? 'SearchableName';
}

export function getSortDirection(): string {
  const element = <HTMLInputElement>document.getElementById('select-sort-direction');
  return element?.value ?? 'Ascending';
}
