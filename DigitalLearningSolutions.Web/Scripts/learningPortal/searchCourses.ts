import { filter } from 'fuzzy';

interface CourseCard {
  element: Element;
  title: string;
}

export function getCourseCards() {
  const courseCardElements = Array.from(document.getElementsByClassName('current-course-card'));
  return courseCardElements.map((element) => ({
    element,
    title: titleFromCardElement(element),
  }));
}

function setUpSearch() {
  const courseCards = getCourseCards();

  const searchInput = <HTMLInputElement>document.getElementById('search-field');
  searchInput?.addEventListener(
    'input',
    (event) => search((<HTMLInputElement>event.target).value, courseCards),
  );
  searchInput?.addEventListener(
    'keydown',
    (event) => {
      if (event.key === 'Enter') {
        event.preventDefault();
      }
    }
  );
}

export function titleFromCardElement(cardElement: Element): string {
  const titleSpan = <HTMLSpanElement>cardElement.getElementsByClassName('nhsuk-details__summary-text')[0];
  return titleSpan.textContent ?? '';
}

export function search(query: string, courseCards: CourseCard[]) {
  if (query.length === 0) {
    clearSearch(courseCards);
    return;
  }

  const options = {
    extract: (courseCard: CourseCard) => courseCard.title,
  };
  const results = filter(query, courseCards, options);

  updateResultCount(results.length);

  const newElements = results.map((res) => res.original.element);
  displayCards(newElements);
}

export function displayCards(courseCardElements: Element[]) {
  const cardContainer = <HTMLDivElement>document.getElementById('current-course-cards');
  cardContainer.textContent = '';
  courseCardElements.forEach((el) => cardContainer.appendChild(el));
}

export function updateResultCount(count: number) {
  const resultCount = <HTMLSpanElement>document.getElementById('results-count');
  resultCount.hidden = false;
  resultCount.setAttribute('aria-hidden', 'false');
  resultCount.textContent = count === 1 ? '1 matching result' : `${count.toString()} matching results`;
}

export function hideResultCount() {
  const resultCount = <HTMLSpanElement>document.getElementById('results-count');
  resultCount.hidden = true;
  resultCount.setAttribute('aria-hidden', 'true');
}

function clearSearch(courseCards: CourseCard[]) {
  const originalElements = courseCards.map((card) => card.element);
  displayCards(originalElements);
  hideResultCount();
}

setUpSearch();
