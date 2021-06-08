import Details from 'nhsuk-frontend/packages/components/details/details';
import { search, setUpSearch } from './search';
import { setupSort, sortSearchableElements } from './sort';
import { ITEMS_PER_PAGE, paginateResults, setupPagination } from './paginate';

export interface SearchableElement {
  element: Element;
  title: string;
}

export class SearchSortAndPaginate {
  private page: number;

  constructor(route: string) {
    this.page = 1;
    SearchSortAndPaginate.getSearchableElements(route).then((allSearchableElements) => {
      if (allSearchableElements === undefined) {
        return;
      }

      setUpSearch(() => this.onSearchUpdated(allSearchableElements));
      setupSort(() => this.searchSortAndPaginate(allSearchableElements));
      setupPagination(
        () => this.onNextPagePressed(allSearchableElements),
        () => this.onPreviousPagePressed(allSearchableElements),
      );
      this.searchSortAndPaginate(allSearchableElements);
    });
  }

  private onSearchUpdated(allSearchableElements: SearchableElement[]): void {
    this.page = 1;
    this.searchSortAndPaginate(allSearchableElements);
  }

  private onNextPagePressed(allSearchableElements: SearchableElement[]): void {
    this.page += 1;
    this.searchSortAndPaginate(allSearchableElements);
  }

  private onPreviousPagePressed(allSearchableElements: SearchableElement[]): void {
    this.page -= 1;
    this.searchSortAndPaginate(allSearchableElements);
  }

  private searchSortAndPaginate(searchableElements: SearchableElement[]): void {
    const filteredElements = search(searchableElements);
    const sortedElements = sortSearchableElements(filteredElements);
    const totalPages = Math.ceil(sortedElements.length / ITEMS_PER_PAGE);
    const paginatedElements = paginateResults(sortedElements, this.page, totalPages);
    SearchSortAndPaginate.displaySearchableElements(paginatedElements);
  }

  static getSearchableElements(route: string): Promise<SearchableElement[] | undefined> {
    return SearchSortAndPaginate.fetchAllSearchableElements(route)
      .then((response): SearchableElement[] | undefined => {
        if (response === null) {
          return undefined;
        }

        const searchableElements = Array.from(response.getElementsByClassName('searchable-element'));
        return searchableElements.map((element) => ({
          element,
          title: SearchSortAndPaginate.titleFromElement(element),
        }));
      });
  }

  private static fetchAllSearchableElements(route: string): Promise<Document | null> {
    return new Promise((res) => {
      const request = new XMLHttpRequest();

      request.onload = () => {
        res(request.responseXML);
      };

      request.open('GET', route, true);
      request.responseType = 'document';
      request.send();
    });
  }

  static titleFromElement(element: Element): string {
    const titleSpan = <HTMLSpanElement>element.getElementsByClassName('searchable-element-title')[0];
    return titleSpan?.textContent ?? '';
  }

  static displaySearchableElements(searchableElements: SearchableElement[]): void {
    const searchableElementsContainer = document.getElementById('searchable-elements');
    if (!searchableElementsContainer) {
      return;
    }
    searchableElementsContainer.textContent = '';
    searchableElements.forEach((searchableElement) => searchableElementsContainer.appendChild(searchableElement.element));
    // This is required to polyfill the new elements in IE
    Details();
  }
}
