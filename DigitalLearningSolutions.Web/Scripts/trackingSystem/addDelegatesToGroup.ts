import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const groupIdElement = <HTMLSelectElement>document.getElementById('selected-group-Id');
const groupId = groupIdElement?.value.trim();
// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`TrackingSystem/Delegates/Groups/AddDelegateToGroup/AddDelegateToGroupItems/${groupId}`, true, true, true, 'AddDelegateToGroupFilter');
