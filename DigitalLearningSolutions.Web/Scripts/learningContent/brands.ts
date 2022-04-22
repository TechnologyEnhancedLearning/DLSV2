import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const brandIdHidden = <HTMLInputElement>document.getElementById('brand-id');
const brandId = brandIdHidden?.value.trim();

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`Home/LearningContent/${brandId}/AllBrandCourses`,
  false,
  true,
  true,
  'BrandCoursesFilter',
  undefined,
  'brandId',
  undefined,
  'courses-heading');
