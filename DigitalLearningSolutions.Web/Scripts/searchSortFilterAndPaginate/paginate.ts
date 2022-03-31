import { ISearchableElement } from './searchSortFilterAndPaginate';

export const ITEMS_PER_PAGE_DEFAULT = 10;

export function setUpPagination(
  onNextPressed: VoidFunction,
  onPreviousPressed: VoidFunction,
  onItemsPerPageUpdated: VoidFunction,
): void {
  const previousButton = getPreviousButton();
  const nextButton = getNextButton();
  const itemsPerPageSelect = getItemsPerPageSelect();
  if (itemsPerPageSelect !== null) {
    itemsPerPageSelect.addEventListener('change', onItemsPerPageUpdated);
  }

  if (previousButton === null || nextButton === null) {
    return;
  }

  previousButton.addEventListener('click',
    (event) => {
      event.preventDefault();
      onPreviousPressed();
    });
  nextButton.addEventListener('click',
    (event) => {
      event.preventDefault();
      onNextPressed();
    });
}

export function paginateResults(
  results: ISearchableElement[],
  page: number,
): ISearchableElement[] {
  const itemsPerPage = getItemsPerPageValue();
  const totalPages = Math.ceil(results.length / itemsPerPage);
  updatePageNumber(page, totalPages);
  updatePageButtonVisibility(page, totalPages);
  return results.slice((page - 1) * itemsPerPage, page * itemsPerPage);
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

export function getItemsPerPageValue() : number {
  const itemsPerPageSelect = getItemsPerPageSelect();
  return itemsPerPageSelect !== null
    ? parseInt((itemsPerPageSelect as HTMLSelectElement).value, 10)
    : ITEMS_PER_PAGE_DEFAULT;
}

function getPreviousButton() {
  return document.getElementsByClassName('nhsuk-pagination__link--prev').item(0);
}

function getNextButton() {
  return document.getElementsByClassName('nhsuk-pagination__link--next').item(0);
}

function getItemsPerPageSelect() {
  return document.getElementById('items-per-page-select');
}

function getPreviousButtonDisplayContainer() {
  return document.getElementsByClassName('nhsuk-pagination-item--previous').item(0) as HTMLLIElement;
}

function getNextButtonDisplayContainer() {
  return document.getElementsByClassName('nhsuk-pagination-item--next').item(0) as HTMLLIElement;
}

function getPaginationDisplayContainer() {
  return document.getElementsByClassName('nhsuk-pagination').item(0) as HTMLLIElement;
}
