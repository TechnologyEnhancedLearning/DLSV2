import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import * as Checkboxes from '../checkboxes';

const selectedElements = document.querySelectorAll('.delegate-checkbox:checked') as NodeListOf<HTMLInputElement>;
const selectedIds = Array.from(selectedElements).map((el) => el.value);
const queryParams = selectedIds.map((id, index) => `selectedIds[${index}]=${id}`);
const route = `TrackingSystem/Delegates/Email/AllEmailDelegateItems?${queryParams.join('&')}`;
const checkboxSelector = '.delegate-checkbox';

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(route, false, false, true, 'EmailDelegateFilter');
setUpSelectAndDeselectButtons();

function alertResultCount(): void {
  const resultCount = document.getElementById('results-count') as HTMLSpanElement;
  resultCount.innerHTML = getModifiedResultCountMessageForScreenReader(resultCount);
}

function setUpSelectAndDeselectButtons(): void {
  const selectAllForm = document.getElementById('select-all-form') as HTMLFormElement;
  const selectAllButton = document.getElementById('select-all-button') as HTMLButtonElement;
  const deselectAllButton = document.getElementById('deselect-all-button') as HTMLButtonElement;

  selectAllForm.addEventListener('submit', (e) => e.preventDefault());

  selectAllButton.addEventListener('click',
    () => {
      Checkboxes.default.selectAll(checkboxSelector);
      alertResultCount();
    });

  deselectAllButton.addEventListener('click',
    () => {
      Checkboxes.default.deselectAll(checkboxSelector);
      alertResultCount();
    });
}

function getModifiedResultCountMessageForScreenReader(
  resultsCountElement: HTMLSpanElement,
): string {
  const resultCountMessage = resultsCountElement.innerHTML;
  const indexOfSpace = resultCountMessage.search('&nbsp');

  if (indexOfSpace === -1) {
    return `${resultCountMessage}&nbsp`;
  }
  return resultCountMessage.substring(0, indexOfSpace);
}
