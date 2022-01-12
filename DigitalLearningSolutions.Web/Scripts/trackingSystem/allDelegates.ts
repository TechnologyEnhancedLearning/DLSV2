import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate('TrackingSystem/Delegates/All/AllDelegateItems',
  true,
  true,
  true,
  'DelegateFilter',
  ['title', 'candidate-number']);
