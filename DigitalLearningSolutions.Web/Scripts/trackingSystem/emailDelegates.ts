import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import { selectAll, deselectAll } from '../common';

const selectedElements = document.querySelectorAll('.delegate-checkbox:checked') as NodeListOf<HTMLInputElement>;
const selectedIds = Array.from(selectedElements).map((el) => el.value);
const queryParams = selectedIds.map((id, index) => `selectedIds[${index}]=${id}`);
const route = `TrackingSystem/Delegates/Email/AllEmailDelegateItems?${queryParams.join('&')}`;
const checkboxSelector = '.delegate-checkbox';

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(route, false, false, true, 'EmailDelegateFilter');

const selectAllForm = document.getElementById('select-all-form') as HTMLFormElement;
const selectAllButton = document.getElementById('select-all-button') as HTMLButtonElement;
const deselectAllButton = document.getElementById('deselect-all-button') as HTMLButtonElement;

selectAllForm.addEventListener('submit', (e) => e.preventDefault());
selectAllButton.addEventListener('click', () => selectAll(checkboxSelector));
deselectAllButton.addEventListener('click', () => deselectAll(checkboxSelector));
