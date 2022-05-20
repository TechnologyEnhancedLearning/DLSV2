import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const brandIdHidden = <HTMLInputElement>document.getElementById('brand-id');
const brandId = brandIdHidden?.value.trim();

const filterEnabled = document.getElementsByClassName('filter-container').length > 0;

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`Home/LearningContent/${brandId}/AllBrandCourses`,
  false,
  true,
  filterEnabled,
  'BrandCoursesFilter',
  undefined,
  undefined,
  undefined,
  'courses-heading');
