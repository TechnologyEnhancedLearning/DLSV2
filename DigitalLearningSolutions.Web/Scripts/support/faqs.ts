import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

(function initiateFaqsSearchAndPagination(): void {
  const subApplication = (<HTMLSpanElement>document.getElementById('dls-sub-application')).innerText.trim();

  // eslint-disable-next-line no-new
  new SearchSortFilterAndPaginate(`${subApplication}/Support/FAQs/AllItems`,
    true,
    true,
    false,
    undefined,
    ['title', 'content']);
}());
