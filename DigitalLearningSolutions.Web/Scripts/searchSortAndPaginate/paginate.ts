import { SearchableElement } from './searchSortAndPaginate';

export const ITEMS_PER_PAGE = 10;

export function setupPagination(
  onNextPressed: VoidFunction,
  onPreviousPressed: VoidFunction,
): void {
  const previousButton = getPreviousButton();
  const nextButton = getNextButton();
  if (previousButton === null || nextButton === null) {
    return;
  }

  previousButton.addEventListener('click', (event) => {
    event.preventDefault();
    onPreviousPressed();
  });
  nextButton.addEventListener('click', (event) => {
    event.preventDefault();
    onNextPressed();
  });
}

export function paginateResults(
  results: SearchableElement[],
  page: number,
  totalPages: number,
): SearchableElement[] {
  updatePageNumber(page, totalPages);
  updatePageButtonVisibility(page, totalPages);
  return results.slice((page - 1) * ITEMS_PER_PAGE, page * ITEMS_PER_PAGE);
}

function updatePageNumber(page: number, totalPages: number) {
  const pageIndicator = document.getElementById('page-indicator');
  if (pageIndicator === null) {
    return;
  }

  if (totalPages > 1) {
    pageIndicator.hidden = false;
    pageIndicator.textContent = `${page} of ${totalPages}`;
  } else {
    pageIndicator.hidden = true;
  }
}

function updatePageButtonVisibility(page: number, totalPages: number) {
  const previousButton = getPreviousButtonDisplayContainer();
  const nextButton = getNextButtonDisplayContainer();
  const paginationContainer = getPaginationDisplayContainer();
  if (previousButton === null || nextButton === null || paginationContainer === null) {
    return;
  }

  nextButton.hidden = page >= totalPages;
  previousButton.hidden = page === 1;
  paginationContainer.style.display = (totalPages === 1 ? 'none' : 'block');
  paginationContainer.hidden = totalPages === 1;
}

function getPreviousButton() {
  return document.getElementsByClassName('nhsuk-pagination__link--prev').item(0);
}

function getNextButton() {
  return document.getElementsByClassName('nhsuk-pagination__link--next').item(0);
}

function getPreviousButtonDisplayContainer() {
  return <HTMLLIElement>document.getElementsByClassName('nhsuk-pagination-item--previous').item(0);
}

function getNextButtonDisplayContainer() {
  return <HTMLLIElement>document.getElementsByClassName('nhsuk-pagination-item--next').item(0);
}

function getPaginationDisplayContainer() {
  return <HTMLLIElement>document.getElementsByClassName('nhsuk-pagination').item(0);
}
