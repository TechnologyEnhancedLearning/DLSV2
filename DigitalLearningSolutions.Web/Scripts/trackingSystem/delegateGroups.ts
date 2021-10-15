import {SearchSortFilterAndPaginate} from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import {getAndTrimPageNumber} from "../common";

const currentPage = getAndTrimPageNumber();

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate('TrackingSystem/Delegates/Groups/AllDelegateGroups', true, true, true, 'DelegateGroupsFilter', currentPage);
