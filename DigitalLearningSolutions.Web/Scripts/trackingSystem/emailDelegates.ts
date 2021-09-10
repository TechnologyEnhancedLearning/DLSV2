import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const selectedElements = document.querySelectorAll('.delegate-checkbox:checked') as NodeListOf<HTMLInputElement>;
const selectedIds = Array.from(selectedElements).map((el) => el.value);
const queryString = '?selectedIds='.concat(selectedIds.join('&selectedIds='));
const route = `TrackingSystem/Delegates/Email/AllEmailDelegateItems${queryString}`;

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(route, true, 'EmailDelegateFilter', false, false, false);

const selectAll = () => {
  const allCheckboxes = document.querySelectorAll('.delegate-checkbox') as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => { if (!checkbox.checked) checkbox.click(); });
};
const deselectAll = () => {
  const allCheckboxes = document.querySelectorAll('.delegate-checkbox') as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => { if (checkbox.checked) checkbox.click(); });
};
const selectAllButton = document.getElementById('select-all-button') as HTMLButtonElement;
const deselectAllButton = document.getElementById('deselect-all-button') as HTMLButtonElement;
selectAllButton.addEventListener('click', selectAll);
deselectAllButton.addEventListener('click', deselectAll);
