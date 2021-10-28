import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const customisationId = document.getElementById('selected-customisation-Id')?.innerHTML.trim();

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`TrackingSystem/Delegates/CourseDelegates/AllCourseDelegates/${customisationId}`, false, true, true, 'CourseDelegatesFilter');
