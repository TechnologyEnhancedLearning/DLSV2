import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import { getQuery } from '../searchSortFilterAndPaginate/search';
import { getExistingFilterStringValue } from '../searchSortFilterAndPaginate/filter';
import { getSortBy, getSortDirection } from '../searchSortFilterAndPaginate/sort';
import getPathForEndpoint from '../common';

const exportCurrentLink = <HTMLAnchorElement>document.getElementById('export');
exportCurrentLink.addEventListener('click', () => {
  const searchString = getQuery();
  const existingFilterString = getExistingFilterStringValue();
  const sortBy = getSortBy();
  const sortDirection = getSortDirection();
  const pathWithCurrentSortFilter = getPathForEndpoint(`TrackingSystem/Delegates/All/Export?searchString=${searchString}&sortBy=${sortBy}&sortDirection=${sortDirection}&existingFilterString=${existingFilterString}`);
  exportCurrentLink.href = pathWithCurrentSortFilter;
});

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(
  'TrackingSystem/Delegates/All/AllDelegateItems',
  true,
  true,
  true,
  'DelegateFilter',
  ['title', 'email', 'candidate-number'],
);
