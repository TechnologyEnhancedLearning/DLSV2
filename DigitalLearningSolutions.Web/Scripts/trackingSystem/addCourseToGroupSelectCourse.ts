import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const currentGroupHidden = <HTMLInputElement>document.getElementById('current-group-id');
const groupId = currentGroupHidden?.value.trim();

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`TrackingSystem/Delegates/Groups/${groupId}/Courses/AddCourseToGroupSelectCourseAllCourses`, true, true, true, 'GroupAddCourseFilter');
