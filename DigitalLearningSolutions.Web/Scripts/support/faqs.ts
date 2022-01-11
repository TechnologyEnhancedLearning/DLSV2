import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

(function initiateFaqsSearchAndPagination(): void {
  const subApplication = getDlsSubApplicationFromFaqUrl();

  // eslint-disable-next-line no-new
  new SearchSortFilterAndPaginate(`${subApplication}/Support/FAQs/AllItems`,
    true,
    true,
    false,
    undefined,
    ['title', 'content']);
}());

function getDlsSubApplicationFromFaqUrl(): string {
  const pathMatchResults = window.location.pathname.match(/^\/(?<SubApplication>\w+)\/Support\/FAQs#?$/);
  return pathMatchResults?.groups?.SubApplication ?? '';
}
