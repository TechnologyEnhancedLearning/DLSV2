import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

(function dlsSupportFaqsHelper(): void {
  const matches = window.location.pathname.match(/^\/(\w+)\/Support\/FAQs#?$/);

  if (!matches || matches.length < 2) {
    return;
  }

  // eslint-disable-next-line no-new
  new SearchSortFilterAndPaginate(`${matches[1]}/Support/FAQs/AllItems`, true, true, false, undefined, ['title', 'content']);
})();
