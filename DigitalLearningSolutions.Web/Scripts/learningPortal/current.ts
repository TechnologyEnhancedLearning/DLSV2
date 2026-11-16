import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

// An empty route makes the client-side controls use the items already rendered in this page.
// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(
  '',
  true,
  true,
  false,
  '',
  ['title'],
  '',
  () => undefined,
  '',
  'current-items-for-javascript',
);
