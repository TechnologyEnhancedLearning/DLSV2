import * as JsSearch from 'js-search';
import { SearchableElement } from './searchSortAndPaginate';

export function setUpSearch(onSearchUpdated: VoidFunction): void {
  const searchInput = getSearchBox();
  searchInput?.addEventListener('input', onSearchUpdated);
  searchInput?.addEventListener(
    'keydown',
    (event) => {
      if (event.key === 'Enter') {
        event.preventDefault();
      }
    }
  );
}

export function search(searchableElements: SearchableElement[]): SearchableElement[] {
  const query = getQuery();
  if (query.length === 0) {
    hideResultCount();
    return searchableElements;
  }

  const searchEngine = new JsSearch.Search(['element', 'id']);
  searchEngine.searchIndex = new JsSearch.UnorderedSearchIndex();
  searchEngine.indexStrategy = new JsSearch.AllSubstringsIndexStrategy();
  searchEngine.addIndex('title');
  searchEngine.addDocuments(searchableElements);
  const results = <SearchableElement[]>searchEngine.search(query);
  updateResultCount(results.length);
  return results;
}

export function updateResultCount(count: number): void {
  const resultCount = <HTMLSpanElement>document.getElementById('results-count');
  resultCount.hidden = false;
  resultCount.setAttribute('aria-hidden', 'false');
  resultCount.textContent = count === 1 ? '1 matching result' : `${count.toString()} matching results`;
}

export function hideResultCount(): void {
  const resultCount = <HTMLSpanElement>document.getElementById('results-count');
  resultCount.hidden = true;
  resultCount.setAttribute('aria-hidden', 'true');
}

function getQuery() {
  const searchBox = getSearchBox();
  return searchBox.value;
}

function getSearchBox() {
  return <HTMLInputElement>document.getElementById('search-field');
}
