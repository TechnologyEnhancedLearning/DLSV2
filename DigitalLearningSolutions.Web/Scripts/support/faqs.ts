import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

(function dlsSupportFaqsHelper(): void {
  const pathMatchResults = window.location.pathname.match(/^\/(?<SubApplication>\w+)\/Support\/FAQs#?$/);
  if (pathMatchResults === null) {
    return;
  }
  const subApplication = pathMatchResults.groups?.SubApplication;
  // eslint-disable-next-line no-new
  new SearchSortFilterAndPaginate(`${subApplication}/Support/FAQs/AllItems`,
    true,
    true,
    false,
    undefined,
    ["title", "content"]);
}());
