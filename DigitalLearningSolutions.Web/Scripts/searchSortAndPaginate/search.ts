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
    return searchableElements;
  }

  const searchEngine = new JsSearch.Search(['element', 'id']);
  searchEngine.searchIndex = new JsSearch.UnorderedSearchIndex();
  searchEngine.indexStrategy = new JsSearch.AllSubstringsIndexStrategy();
  searchEngine.addIndex('title');
  searchEngine.addDocuments(searchableElements);
  const results = <SearchableElement[]>searchEngine.search(query);
  return results;
}

export function getQuery(): string {
  const searchBox = getSearchBox();
  return searchBox.value;
}

function getSearchBox(): HTMLInputElement {
  return <HTMLInputElement>document.getElementById('search-field');
}
