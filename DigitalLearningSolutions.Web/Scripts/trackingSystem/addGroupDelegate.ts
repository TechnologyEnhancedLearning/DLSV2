import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const groupIdElement = <HTMLSelectElement>document.getElementById('selected-group-Id');
const groupId = groupIdElement?.value.trim();
// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`TrackingSystem/Delegates/Groups/${groupId}/Delegates/Add/SelectDelegate/AllItems`,
  true,
  true,
  true,
  'AddGroupDelegateFilter',
  ['title', 'email', 'candidate-number']);
