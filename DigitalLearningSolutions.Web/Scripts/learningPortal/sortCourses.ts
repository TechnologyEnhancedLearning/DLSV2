import moment from 'moment';
import * as _ from 'lodash';

export default function onSortCriteriaChange() {
  sortCards(getSortBy(), getSortDirection());
}

export function sortCards(sortBy: string, sortDirection: string) {
  const courseCardsContainer = document.getElementById('course-cards');
  if (!courseCardsContainer) {
    return;
  }
  const courseCards = Array.from(courseCardsContainer.children);

  sortAndDisplayCards(courseCards, sortBy, sortDirection);
}

export function sortAndDisplaySearchResults(courseCards: Element[]) {
  sortAndDisplayCards(courseCards, getSortBy(), getSortDirection());
}

function sortAndDisplayCards(
  courseCards: Element[],
  sortBy: string,
  sortDirection: string,
) {
  const courseCardsContainer = document.getElementById('course-cards');
  if (!courseCardsContainer) {
    return;
  }
  courseCardsContainer.textContent = '';

  const sortedCards = _.orderBy(
    courseCards,
    [(course: Element) => getSortValue(course, sortBy)],
    [(sortDirection === 'Descending') ? 'desc' : 'asc'],
  );
  sortedCards.forEach((element) => courseCardsContainer.appendChild(element));
}

export function getSortValue(courseCard: Element, sortBy: string): string | number | Date {
  switch (sortBy) {
    case 'Course Name':
      return courseCard.querySelector('[name="name"]')?.textContent?.trim() ?? '';
    case 'Enrolled Date':
      return parseDate(courseCard.querySelector('[name="started-date"]')?.innerHTML || '');
    case 'Last Accessed Date':
      return parseDate(courseCard.querySelector('[name="accessed-date"]')?.innerHTML || '');
    case 'Complete By Date':
      return parseDate(courseCard.querySelector('[name="complete-by-date"]')?.innerHTML || '');
    case 'Completed Date':
      return parseDate(courseCard.querySelector('[name="completed-date"]')?.innerHTML || '');
    case 'Diagnostic Score':
      return parseInt(courseCard.querySelector('[name="diagnostic-score"]')?.innerHTML.split('/')[0] || '-1', 10);
    case 'Passed Sections':
      return parseInt(courseCard.querySelector('[name="passed-sections"]')?.innerHTML.split('/')[0] || '-1', 10);
    default:
      return '';
  }
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

function registerListeners() {
  document.getElementById('select-sort-by')?.addEventListener('change', onSortCriteriaChange);
  document.getElementById('select-sort-direction')?.addEventListener('change', onSortCriteriaChange);
}

registerListeners();
