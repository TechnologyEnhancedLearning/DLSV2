import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const courseSelectBox = <HTMLSelectElement>document.getElementById('selected-customisation-Id');
const customisationId = courseSelectBox?.value.trim();
// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`TrackingSystem/Delegates/CourseDelegates/AllCourseDelegates/${customisationId}`, false, true, true, 'CourseDelegatesFilter');
