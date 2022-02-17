import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import { getFilterByValue } from '../searchSortFilterAndPaginate/filter';
import { getSortBy, getSortDirection } from '../searchSortFilterAndPaginate/sort';
import getPathForEndpoint from '../common';

const courseSelectBox = <HTMLSelectElement>document.getElementById('selected-customisation-Id');
const customisationId = courseSelectBox?.value.trim();

const exportCurrentLink = <HTMLAnchorElement>document.getElementById('export-current');
exportCurrentLink.addEventListener('click', (e) => {
  const filterBy = getFilterByValue();
  const sortBy = getSortBy();
  const sortDirection = getSortDirection();
  const pathWithCurrentSortFilter = getPathForEndpoint(`TrackingSystem/Delegates/CourseDelegates/DownloadCurrent/${customisationId}?sortBy=${sortBy}&sortDirection=${sortDirection}&filterBy=${filterBy}`);
  exportCurrentLink.href = pathWithCurrentSortFilter;
});

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`TrackingSystem/Delegates/CourseDelegates/AllCourseDelegates/${customisationId}`, false, true, true, 'CourseDelegatesFilter');
