import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import { getExistingFilterStringValue } from '../searchSortFilterAndPaginate/filter';
import { getSortBy, getSortDirection } from '../searchSortFilterAndPaginate/sort';
import getPathForEndpoint from '../common';

const courseSelectBox = <HTMLSelectElement>document.getElementById('selected-customisation-Id');
const customisationId = courseSelectBox?.value.trim();

const exportCurrentLink = <HTMLAnchorElement>document.getElementById('export');
exportCurrentLink.addEventListener('click', () => {
  const existingFilterString = getExistingFilterStringValue();
  const sortBy = getSortBy();
  const sortDirection = getSortDirection();
  const pathWithCurrentSortFilter = getPathForEndpoint(`TrackingSystem/Delegates/CourseDelegates/DownloadCurrent/${customisationId}?sortBy=${sortBy}&sortDirection=${sortDirection}&existingFilterString=${existingFilterString}`);
  exportCurrentLink.href = pathWithCurrentSortFilter;
});

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`TrackingSystem/Delegates/CourseDelegates/AllCourseDelegates/${customisationId}`, false, true, true, 'CourseDelegatesFilter', undefined, 'CUSTOMISATIONID');
