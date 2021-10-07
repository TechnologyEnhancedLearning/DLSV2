import {SearchSortFilterAndPaginate} from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import {getLastRouteParam} from "../common";

const currentPage = parseInt(getLastRouteParam());

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate('TrackingSystem/Delegates/Groups/AllDelegateGroups', true, true, true, 'DelegateGroupsFilter', currentPage);
