import Details from 'nhsuk-frontend/packages/components/details/details';
import {
  setUpFilter, filterSearchableElements, getFilterByValue, IAppliedFilterTag,
} from './filter';
import { getQuery, search, setUpSearch } from './search';
import { setUpSort, sortSearchableElements } from './sort';
import { ITEMS_PER_PAGE, paginateResults, setUpPagination } from './paginate';
import getPathForEndpoint from '../common';

export interface ISearchableElement {
  element: Element;
  title: string;
}

export interface ISearchableData {
  searchableElements: ISearchableElement[];
  possibleFilters: IAppliedFilterTag[];
}

export class SearchSortFilterAndPaginate {
  private page: number;

  private readonly filterEnabled: boolean;

  private readonly searchEnabled: boolean;

  private readonly sortEnabled: boolean;

  private readonly paginationEnabled: boolean;

  // Route proved should be a relative path with no leading /
  constructor(route: string, filterEnabled = false, filterCookieName = '', searchEnabled = true, sortEnabled = true, paginationEnabled = true) {
    this.page = 1;
    this.filterEnabled = filterEnabled;
    this.searchEnabled = searchEnabled;
    this.sortEnabled = sortEnabled;
    this.paginationEnabled = paginationEnabled;

    SearchSortFilterAndPaginate.getSearchableElements(route).then((searchableData) => {
      if (searchableData === undefined) {
        return;
      }

      if (filterEnabled) {
        setUpFilter(() => this.onFilterUpdated(searchableData), filterCookieName);
      }
      if (searchEnabled) {
        setUpSearch(() => this.onSearchUpdated(searchableData));
      }
      if (sortEnabled) {
        setUpSort(() => this.searchSortAndPaginate(searchableData));
      }
      if (paginationEnabled) {
        setUpPagination(
          () => this.onNextPagePressed(searchableData),
          () => this.onPreviousPagePressed(searchableData),
        );
      }
      this.searchSortAndPaginate(searchableData);
    });
  }

  private onFilterUpdated(searchableData: ISearchableData): void {
    this.page = 1;
    this.searchSortAndPaginate(searchableData);
  }

  private onSearchUpdated(searchableData: ISearchableData): void {
    this.page = 1;
    this.searchSortAndPaginate(searchableData);
  }

  private onNextPagePressed(searchableData: ISearchableData): void {
    this.page += 1;
    this.searchSortAndPaginate(searchableData);
  }

  private onPreviousPagePressed(searchableData: ISearchableData): void {
    this.page -= 1;
    this.searchSortAndPaginate(searchableData);
  }

  private searchSortAndPaginate(searchableData: ISearchableData): void {
    const searchedElements = this.searchEnabled
      ? search(searchableData.searchableElements)
      : searchableData.searchableElements;
    const filteredElements = this.filterEnabled
      ? filterSearchableElements(searchedElements, searchableData.possibleFilters)
      : searchedElements;
    const sortedElements = this.sortEnabled
      ? sortSearchableElements(filteredElements)
      : filteredElements;

    if (this.shouldDisplayResultCount()) {
      SearchSortFilterAndPaginate.updateResultCount(sortedElements.length);
    } else {
      SearchSortFilterAndPaginate.hideResultCount();
    }

    const totalPages = Math.ceil(sortedElements.length / ITEMS_PER_PAGE);
    const paginatedElements = this.paginationEnabled
      ? paginateResults(sortedElements, this.page, totalPages)
      : sortedElements;
    SearchSortFilterAndPaginate.displaySearchableElements(paginatedElements);
  }

  static getSearchableElements(route: string): Promise<ISearchableData | undefined> {
    return SearchSortFilterAndPaginate.fetchAllSearchableElements(route)
      .then((response): ISearchableData | undefined => {
        if (response === null) {
          return undefined;
        }

        const elements = Array.from(response.getElementsByClassName('searchable-element'));
        const searchableElements = elements.map((element) => ({
          element,
          title: SearchSortFilterAndPaginate.titleFromElement(element),
        }));
        const tags = Array.from(response.getElementsByClassName('filter-tag'));
        const possibleAppliedFilters = tags.map((element) => ({
          element,
          filterValue: SearchSortFilterAndPaginate.filterValueFromElement(element),
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

  static displaySearchableElements(searchableElements: ISearchableElement[]): void {
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
    const filterString = this.filterEnabled ? getFilterByValue() : false;
    const searchString = this.searchEnabled ? getQuery() : false;
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
