import Details from 'nhsuk-frontend/packages/components/details/details';
import {
  setupFilter, filterSearchableElements, getFilterBy, AppliedFilterTag,
} from './filter';
import { getQuery, search, setUpSearch } from './search';
import { setupSort, sortSearchableElements } from './sort';
import { ITEMS_PER_PAGE, paginateResults, setupPagination } from './paginate';
import getPathForEndpoint from '../common';

export interface SearchableElement {
  element: Element;
  title: string;
}

export interface SearchableData {
  searchableElements: SearchableElement[];
  possibleFilters: AppliedFilterTag[];
}

export class SearchSortAndPaginate {
  private page: number;

  private readonly filterEnabled: boolean;

  // Route proved should be a relative path with no leading /
  constructor(route: string, filterEnabled = false) {
    this.page = 1;
    this.filterEnabled = filterEnabled;

    SearchSortAndPaginate.getSearchableElements(route).then((searchableData) => {
      if (searchableData === undefined) {
        return;
      }

      if (filterEnabled) {
        setupFilter(() => this.onFilterUpdated(searchableData));
      }

      setUpSearch(() => this.onSearchUpdated(searchableData));
      setupSort(() => this.searchSortAndPaginate(searchableData));
      setupPagination(
        () => this.onNextPagePressed(searchableData),
        () => this.onPreviousPagePressed(searchableData),
      );
      this.searchSortAndPaginate(searchableData);
    });
  }

  private onFilterUpdated(searchableData: SearchableData): void {
    this.page = 1;
    this.searchSortAndPaginate(searchableData);
  }

  private onSearchUpdated(searchableData: SearchableData): void {
    this.page = 1;
    this.searchSortAndPaginate(searchableData);
  }

  private onNextPagePressed(searchableData: SearchableData): void {
    this.page += 1;
    this.searchSortAndPaginate(searchableData);
  }

  private onPreviousPagePressed(searchableData: SearchableData): void {
    this.page -= 1;
    this.searchSortAndPaginate(searchableData);
  }

  private searchSortAndPaginate(searchableData: SearchableData): void {
    const searchedElements = search(searchableData.searchableElements);
    const filteredElements = this.filterEnabled
      ? filterSearchableElements(searchedElements, searchableData.possibleFilters)
      : searchedElements;
    const sortedElements = sortSearchableElements(filteredElements);

    if (this.shouldDisplayResultCount()) {
      SearchSortAndPaginate.updateResultCount(sortedElements.length);
    } else {
      SearchSortAndPaginate.hideResultCount();
    }

    const totalPages = Math.ceil(sortedElements.length / ITEMS_PER_PAGE);
    const paginatedElements = paginateResults(sortedElements, this.page, totalPages);
    SearchSortAndPaginate.displaySearchableElements(paginatedElements);
  }

  static getSearchableElements(route: string): Promise<SearchableData | undefined> {
    return SearchSortAndPaginate.fetchAllSearchableElements(route)
      .then((response): SearchableData | undefined => {
        if (response === null) {
          return undefined;
        }

        const elements = Array.from(response.getElementsByClassName('searchable-element'));
        const searchableElements = elements.map((element) => ({
          element,
          title: SearchSortAndPaginate.titleFromElement(element),
        }));
        const tags = Array.from(response.getElementsByClassName('filter-tag'));
        const possibleAppliedFilters = tags.map((element) => ({
          element,
          filterValue: SearchSortAndPaginate.filterValueFromElement(element),
        }));
        return {
          searchableElements,
          possibleFilters: possibleAppliedFilters,
        };
      });
  }

  private static fetchAllSearchableElements(route: string): Promise<Document | null> {
    const path = getPathForEndpoint(route);
    return new Promise((res) => {
      const request = new XMLHttpRequest();

      request.onload = () => {
        res(request.responseXML);
      };

      request.open('GET', path, true);
      request.responseType = 'document';
      request.send();
    });
  }

  static titleFromElement(element: Element): string {
    const titleSpan = <HTMLSpanElement>element.getElementsByClassName('searchable-element-title')[0];
    return titleSpan?.textContent ?? '';
  }

  static filterValueFromElement(element: Element): string {
    return element.getAttribute('data-filter-value')?.trim() ?? '';
  }

  static displaySearchableElements(searchableElements: SearchableElement[]): void {
    const searchableElementsContainer = document.getElementById('searchable-elements');
    if (!searchableElementsContainer) {
      return;
    }
    searchableElementsContainer.textContent = '';
    searchableElements.forEach(
      (searchableElement) => searchableElementsContainer.appendChild(searchableElement.element),
    );
    // This is required to polyfill the new elements in IE
    Details();
  }

  private shouldDisplayResultCount(): boolean {
    const filterString = this.filterEnabled ? getFilterBy() : false;
    const searchString = getQuery();
    return !!(filterString || searchString);
  }

  static updateResultCount(count: number): void {
    const resultCount = <HTMLSpanElement>document.getElementById('results-count');
    resultCount.hidden = false;
    resultCount.setAttribute('aria-hidden', 'false');
    resultCount.textContent = count === 1 ? '1 matching result' : `${count.toString()} matching results`;
  }

  static hideResultCount(): void {
    const resultCount = <HTMLSpanElement>document.getElementById('results-count');
    resultCount.hidden = true;
    resultCount.setAttribute('aria-hidden', 'true');
  }
}
