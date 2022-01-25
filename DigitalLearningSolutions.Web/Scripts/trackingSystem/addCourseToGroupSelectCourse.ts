import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const currentGroupHidden = <HTMLInputElement>document.getElementById('current-group-id');
const groupId = currentGroupHidden?.value.trim();

const currentAdminCategoryHidden = <HTMLInputElement>document.getElementById('current-admin-category-id');
const adminCategory = currentAdminCategoryHidden?.value.trim();
const filterEnabled = adminCategory === '';
// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`TrackingSystem/Delegates/Groups/${groupId}/Courses/AddCourseToGroupSelectCourseAllCourses`, true, true, filterEnabled, 'GroupAddCourseFilter');
