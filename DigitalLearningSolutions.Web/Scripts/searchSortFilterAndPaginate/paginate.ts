import { ISearchableElement } from './searchSortFilterAndPaginate';

export const ITEMS_PER_PAGE_DEFAULT = 10;

export function setUpPagination(
  onNextPressed: VoidFunction,
  onPreviousPressed: VoidFunction,
  onItemsPerPageUpdated: VoidFunction,
): void {
  const previousButtons = getPreviousButtons();
  const nextButtons = getNextButtons();
  const itemsPerPageSelect = getItemsPerPageSelect();

  previousButtons.forEach((button) => {
    button.addEventListener(
      'click',
      (event) => {
        event.preventDefault();
        onPreviousPressed();
      },
    );
  });

  nextButtons.forEach((button) => {
    button.addEventListener(
      'click',
      (event) => {
        event.preventDefault();
        onNextPressed();
      },
    );
  });

  if (itemsPerPageSelect !== null) {
    itemsPerPageSelect.addEventListener('change', onItemsPerPageUpdated);
  }
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
  const pageIndicators = <HTMLSpanElement[]>Array.from(document.getElementsByClassName('page-indicator'));
  pageIndicators.forEach((pageIndicator) => {
    const element = pageIndicator;

    if (totalPages > 1) {
      element.hidden = false;
      element.textContent = `${page} of ${totalPages}`;
    } else {
      element.hidden = true;
    }
  });
}

function updatePageButtonVisibility(page: number, totalPages: number) {
  const previousButtons = getPreviousButtonDisplayContainers();
  const nextButtons = getNextButtonDisplayContainers();
  const paginationContainers = getPaginationDisplayContainers();

  nextButtons.forEach((button) => {
    const element = button;
    element.hidden = page >= totalPages;
  });
  previousButtons.forEach((button) => {
    const element = button;
    element.hidden = page === 1;
  });
  paginationContainers.forEach((container) => {
    const element = container;
    element.style.display = (totalPages === 1 ? 'none' : 'block');
    element.hidden = totalPages === 1;
  });
}

export function getItemsPerPageValue() : number {
  const itemsPerPageSelect = getItemsPerPageSelect();
  return itemsPerPageSelect !== null
    ? parseInt((itemsPerPageSelect as HTMLSelectElement).value, 10)
    : ITEMS_PER_PAGE_DEFAULT;
}

function getPreviousButtons() {
  return Array.from(document.getElementsByClassName('nhsuk-pagination__link--prev'));
}

function getNextButtons() {
  return Array.from(document.getElementsByClassName('nhsuk-pagination__link--next'));
}

function getItemsPerPageSelect() {
  return document.getElementById('items-per-page-select');
}

function getPreviousButtonDisplayContainers() {
  return <HTMLLIElement[]>Array.from(document.getElementsByClassName('nhsuk-pagination-item--previous'));
}

function getNextButtonDisplayContainers() {
  return <HTMLLIElement[]>Array.from(document.getElementsByClassName('nhsuk-pagination-item--next'));
}

function getPaginationDisplayContainers() {
  return <HTMLLIElement[]>Array.from(document.getElementsByClassName('nhsuk-pagination'));
}
