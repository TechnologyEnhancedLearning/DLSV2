import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const selectedElements = document.querySelectorAll('.delegate-checkbox:checked') as NodeListOf<HTMLInputElement>;
const selectedIds = Array.from(selectedElements).map((el) => el.value);
const queryParams = selectedIds.map((id, index) => `selectedIds[${index}]=${id}`);
const route = `TrackingSystem/Delegates/Email/AllEmailDelegateItems?${queryParams.join('&')}`;

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(route, false, false, true, 'EmailDelegateFilter');

function alertResultCount(): void {
  const resultCount = document.getElementById('results-count') as HTMLSpanElement;
  resultCount.setAttribute('role', '');
  resultCount.setAttribute('role', 'alert');
}

function selectAll(): void {
  const allCheckboxes = document.querySelectorAll('.delegate-checkbox') as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => {
    if (!checkbox.checked) checkbox.click();
  });
  alertResultCount();
}

function deselectAll(): void {
  const allCheckboxes = document.querySelectorAll('.delegate-checkbox') as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => {
    if (checkbox.checked) checkbox.click();
  });
  alertResultCount();
}

const selectAllForm = document.getElementById('select-all-form') as HTMLFormElement;
const selectAllButton = document.getElementById('select-all-button') as HTMLButtonElement;
const deselectAllButton = document.getElementById('deselect-all-button') as HTMLButtonElement;

selectAllForm.addEventListener('submit', (e) => e.preventDefault());
selectAllButton.addEventListener('click', selectAll);
deselectAllButton.addEventListener('click', deselectAll);
