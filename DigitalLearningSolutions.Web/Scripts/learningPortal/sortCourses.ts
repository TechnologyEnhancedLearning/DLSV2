import moment from 'moment';
import * as _ from 'lodash';
import { CourseCard } from './searchSortAndPaginate';

export function setupSort(onSortUpdated: VoidFunction): void {
  document.getElementById('select-sort-by')?.addEventListener('change', onSortUpdated);
  document.getElementById('select-sort-direction')?.addEventListener('change', onSortUpdated);
}

export function sortCards(
  courseCards: CourseCard[],
): CourseCard[] {
  const sortBy = getSortBy();
  const sortDirection = getSortDirection();

  return _.orderBy<CourseCard>(
    courseCards,
    [(course) => getSortValue(course, sortBy)],
    [(sortDirection === 'Descending') ? 'desc' : 'asc'],
  );
}

export function getSortValue(courseCard: CourseCard, sortBy: string): string | number | Date {
  switch (sortBy) {
    case 'Activity Name':
      return getElementText(courseCard, 'name').toLocaleLowerCase();
    case 'Enrolled Date':
      return parseDate(getElementText(courseCard, 'started-date'));
    case 'Last Accessed Date':
      return parseDate(getElementText(courseCard, 'accessed-date'));
    case 'Complete By Date':
      return parseDate(getElementText(courseCard, 'complete-by-date'));
    case 'Completed Date':
      return parseDate(getElementText(courseCard, 'completed-date'));
    case 'Diagnostic Score':
      return parseInt(getElementText(courseCard, 'diagnostic-score').split('/')[0] || '-1', 10);
    case 'Passed Sections':
      return parseInt(getElementText(courseCard, 'passed-sections').split('/')[0] || '-1', 10);
    case 'Brand':
      return getElementText(courseCard, 'brand').toLocaleLowerCase();
    case 'Category':
      return getElementText(courseCard, 'category').toLocaleLowerCase();
    case 'Topic':
      return getElementText(courseCard, 'topic').toLocaleLowerCase();
    default:
      return '';
  }
}

function getElementText(courseCard: CourseCard, elementName: string): string {
  return courseCard.element.querySelector(`[name="${elementName}"]`)?.textContent?.trim() ?? '';
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
