import Details from 'nhsuk-frontend/packages/components/details/details';
import _ from 'lodash';
import {
  setUpFilter, filterSearchableElements, IAppliedFilterTag,
} from './filter';
import { getQuery, search, setUpSearch } from './search';
import { setUpSort, sortSearchableElements } from './sort';
import { paginateResults, setUpPagination } from './paginate';
import getPathForEndpoint from '../common';

export interface ISearchableElement {
  parentIndex: number;
  element: Element;
  searchableContent: string;
}

export interface ISearchableData {
  searchableElements: ISearchableElement[];
  possibleFilters: IAppliedFilterTag[];
}

export class SearchSortFilterAndPaginate {
  private page: number;

  private readonly searchEnabled: boolean;

  private readonly paginationEnabled: boolean;

  private readonly filterEnabled: boolean;

  private spinnerContainer: HTMLElement;

  private spinner: HTMLElement;

  private areaToHide: HTMLElement;

  // Route provided should be a relative path with no leading /
  constructor(
    route: string,
    searchEnabled: boolean,
    paginationEnabled: boolean,
    filterEnabled: boolean,
    filterCookieName = '',
    searchableElementClassSuffixes = ['title'],
  ) {
    this.spinnerContainer = document.getElementById('loading-spinner-container') as HTMLElement;
    this.spinner = document.getElementById('dynamic-loading-spinner') as HTMLElement;
    this.areaToHide = document.getElementById('area-to-hide-while-loading') as HTMLElement;
    this.startLoadingSpinner();
    this.page = SearchSortFilterAndPaginate.getPageNumber();
    this.searchEnabled = searchEnabled;
    this.paginationEnabled = paginationEnabled;
    this.filterEnabled = filterEnabled;

    SearchSortFilterAndPaginate.getSearchableElements(route, searchableElementClassSuffixes)
      .then((searchableData) => {
        if (searchableData === undefined) {
          return;
        }

        if (filterEnabled) {
          setUpFilter(() => this.onFilterUpdated(searchableData), filterCookieName);
        }
        if (searchEnabled) {
          setUpSearch(() => this.onSearchUpdated(searchableData));
        }

        setUpSort(() => this.searchSortAndPaginate(searchableData));

        if (paginationEnabled) {
          setUpPagination(
            () => this.onNextPagePressed(searchableData),
            () => this.onPreviousPagePressed(searchableData),
            () => this.onItemsPerPageUpdated(searchableData),
          );
          this.updateSearchableElementLinks(searchableData);
        }
        this.searchSortAndPaginate(searchableData);
        this.stopLoadingSpinner();
      });
  }

  private onFilterUpdated(searchableData: ISearchableData): void {
    this.updatePageNumber(1, searchableData);
    this.searchSortAndPaginate(searchableData);
    SearchSortFilterAndPaginate.scrollToTop();
  }

  private onSearchUpdated(searchableData: ISearchableData): void {
    this.updatePageNumber(1, searchableData);
    this.searchSortAndPaginate(searchableData);
  }

  private onItemsPerPageUpdated(searchableData: ISearchableData): void {
    this.updatePageNumber(1, searchableData);
    this.searchSortAndPaginate(searchableData);
  }

  private onNextPagePressed(searchableData: ISearchableData): void {
    this.updatePageNumber(this.page + 1, searchableData);
    this.searchSortAndPaginate(searchableData);
    SearchSortFilterAndPaginate.scrollToTop();
  }

  private onPreviousPagePressed(searchableData: ISearchableData): void {
    this.updatePageNumber(this.page - 1, searchableData);
    this.searchSortAndPaginate(searchableData);
    SearchSortFilterAndPaginate.scrollToTop();
  }

  private searchSortAndPaginate(searchableData: ISearchableData): void {
    const searchedElements = this.searchEnabled
      ? search(searchableData.searchableElements)
      : searchableData.searchableElements;
    const filteredElements = this.filterEnabled
      ? filterSearchableElements(searchedElements, searchableData.possibleFilters)
      : searchedElements;
    const sortedElements = sortSearchableElements(filteredElements);

    const sortedUniqueElements = _.uniqBy(sortedElements, 'parentIndex');
    const resultCount = sortedUniqueElements.length;
    SearchSortFilterAndPaginate
      .updateResultCount(resultCount);

    const paginatedElements = this.paginationEnabled
      ? paginateResults(sortedUniqueElements, this.page)
      : sortedUniqueElements;
    SearchSortFilterAndPaginate.displaySearchableElements(paginatedElements);
  }

  static getSearchableElements(route: string, searchableElementClassSuffixes: string[]):
    Promise<ISearchableData | undefined> {
    return SearchSortFilterAndPaginate.fetchAllSearchableElements(route)
      .then((response): ISearchableData | undefined => {
        if (response === null) {
          return undefined;
        }

        const elements = Array.from(response.getElementsByClassName('searchable-element'));
        const searchableElements = new Array<ISearchableElement>();

        elements.forEach((element, index) => {
          const searchableItems = searchableElementClassSuffixes
            .map<ISearchableElement>((suffix: string) => ({
              parentIndex: index,
              element,
              searchableContent: SearchSortFilterAndPaginate
                .searchableContentFromElement(element, suffix),
            }));
          searchableElements.push(...searchableItems);
        });
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

  static searchableContentFromElement(element: Element, classSuffix: string): string {
    const searchableContentSpan = <HTMLSpanElement>element.getElementsByClassName(`searchable-element-${classSuffix}`)[0];
    return searchableContentSpan?.textContent ?? '';
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

  static updateResultCount(count: number): void {
    const resultCount = <HTMLSpanElement>document.getElementById('results-count');
    resultCount.hidden = false;
    resultCount.setAttribute('aria-hidden', 'false');
    const newResultCountMessage = this.getNewResultCountMessage(
      count,
      resultCount,
    );

    resultCount.innerHTML = newResultCountMessage;
  }

  static getNewResultCountMessage(
    count: number,
    resultCountElement: HTMLSpanElement,
  ): string {
    const oldResultCountMessage = resultCountElement.innerHTML;
    const newResultCountMessage = count === 1 ? '1 matching result' : `${count.toString()} matching results`;

    if (newResultCountMessage === oldResultCountMessage) {
      // Screen reader does not announce the message if it has not changed
      return `${newResultCountMessage}&nbsp`;
    }

    return newResultCountMessage;
  }

  private static scrollToTop(): void {
    window.scrollTo(0, 0);
  }

  private updatePageNumber(pageNumber: number, searchableData: ISearchableData): void {
    this.page = pageNumber;

    SearchSortFilterAndPaginate.ensurePageNumberSetInUrl();
    const currentPath = window.location.pathname;
    const urlParts = currentPath.split('/');
    const newUrl = `${urlParts.slice(0, -1).join('/')}/${pageNumber.toString()}`;
    window.history.replaceState({}, '', newUrl);

    this.updateSearchableElementLinks(searchableData);
  }

  private static getPageNumber(): number {
    SearchSortFilterAndPaginate.ensurePageNumberSetInUrl();
    const currentPath = window.location.pathname;
    const urlParts = currentPath.split('/');
    return parseInt(urlParts[urlParts.length - 1], 10);
  }

  /* Guarantees the last element of the path is a number with no trailing slashes */
  private static ensurePageNumberSetInUrl(): void {
    const currentPath = window.location.pathname;
    const urlParts = currentPath.split('/');
    if (urlParts[urlParts.length - 1] === '') {
      urlParts.pop();
    }

    const pageNumber = parseInt(urlParts[urlParts.length - 1], 10);

    if (Number.isNaN(pageNumber)) {
      const newUrl = `${urlParts.join('/')}/1`;
      window.history.replaceState({}, '', newUrl);
    }
  }

  private updateSearchableElementLinks(searchableData: ISearchableData): void {
    const setReturnPage = !this.searchEnabled || getQuery().length === 0;

    _.forEach(searchableData.searchableElements, (searchableElement) => {
      _.forEach(searchableElement.element.getElementsByTagName('a'), (anchor: HTMLAnchorElement) => {
        const params = new URLSearchParams(anchor.search);
        if (setReturnPage) {
          params.set('returnPage', this.page.toString());
        } else {
          params.delete('returnPage');
        }
        // eslint-disable-next-line no-param-reassign
        anchor.search = params.toString();
      });
    });
  }

  private startLoadingSpinner(): void {
    this.spinnerContainer?.classList.remove('display-none');
    this.spinner?.classList.remove('loading-spinner');
    this.areaToHide?.classList.add('display-none');
  }

  private stopLoadingSpinner(): void {
      this.spinnerContainer?.classList.add('display-none');
      this.spinner?.classList.add('loading-spinner');
      this.areaToHide?.classList.remove('display-none');
  }
}
