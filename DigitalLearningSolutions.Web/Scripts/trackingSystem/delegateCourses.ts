import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import { getExistingFilterStringValue } from '../searchSortFilterAndPaginate/filter';
import { getSortBy, getSortDirection } from '../searchSortFilterAndPaginate/sort';
import getPathForEndpoint from '../common';
import { getQuery } from '../searchSortFilterAndPaginate/search';

const exportAllLink = <HTMLAnchorElement>document.getElementById('export-all');
exportAllLink.addEventListener('click', () => {
  const searchString = getQuery();
  const existingFilterString = getExistingFilterStringValue();
  const sortBy = getSortBy();
  const sortDirection = getSortDirection();
  exportAllLink.href = getPathForEndpoint(`TrackingSystem/Delegates/Courses/DownloadAll?searchString=${searchString}&sortBy=${sortBy}&sortDirection=${sortDirection}&existingFilterString=${existingFilterString}`);
});

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate('TrackingSystem/Delegates/Courses/AllCourseStatistics', true, true, true, 'DelegateCoursesFilter');
