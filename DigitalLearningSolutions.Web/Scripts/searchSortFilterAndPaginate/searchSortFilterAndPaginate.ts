import Details from 'nhsuk-frontend/packages/components/details/details';
import _ from 'lodash';
import {
  setUpFilter, filterSearchableElements, IAppliedFilterTag,
} from './filter';
import { getQuery, search, setUpSearch } from './search';
import {
  setUpSort, sortSearchableElements, getSortBy, getSortDirection,
} from './sort';
import { paginateResults, setUpPagination, getItemsPerPageValue } from './paginate';
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

  private readonly queryParameterToRetain: string;

  private readonly searchEnabled: boolean;

  private readonly paginationEnabled: boolean;

  private readonly filterEnabled: boolean;

  private spinnerContainer: HTMLElement;

  private spinner: HTMLElement;

  private areaToHide: HTMLElement;

  private readonly functionToRunAfterDisplayingData: VoidFunction;

  // Route provided should be a relative path with no leading /
  constructor(
    route: string,
    searchEnabled: boolean,
    paginationEnabled: boolean,
    filterEnabled: boolean,
    filterCookieName = '',
    searchableElementClassSuffixes = ['title'],
    queryParameterToRetain = '',
    functionToRunAfterDisplayingData: VoidFunction = defaultVoidFunction,
  ) {
    this.spinnerContainer = document.getElementById('loading-spinner-container') as HTMLElement;
    this.spinner = document.getElementById('dynamic-loading-spinner') as HTMLElement;
    this.areaToHide = document.getElementById('js-styling-hidden-area-while-loading') as HTMLElement;
    this.functionToRunAfterDisplayingData = functionToRunAfterDisplayingData;

    this.startLoadingSpinner();
    this.queryParameterToRetain = queryParameterToRetain;
    this.page = paginationEnabled ? this.getPageNumber() : 1;
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

        setUpSort(() => this.onSortUpdated(searchableData));

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
        SearchSortFilterAndPaginate.scrollToLastItemViewed();
      });
  }

  private onFilterUpdated(searchableData: ISearchableData): void {
    this.updatePageNumberIfPaginated(1, searchableData);
    this.searchSortAndPaginate(searchableData);
    SearchSortFilterAndPaginate.scrollToTop();
  }

  private onSortUpdated(searchableData: ISearchableData): void {
    this.updatePageNumberIfPaginated(1, searchableData);
    this.searchSortAndPaginate(searchableData);
  }

  private onSearchUpdated(searchableData: ISearchableData): void {
    this.updatePageNumberIfPaginated(1, searchableData);
    this.searchSortAndPaginate(searchableData);
  }

  private onItemsPerPageUpdated(searchableData: ISearchableData): void {
    this.updatePageNumberIfPaginated(1, searchableData);
    this.searchSortAndPaginate(searchableData);
  }

  private onNextPagePressed(searchableData: ISearchableData): void {
    this.updatePageNumberIfPaginated(this.page + 1, searchableData);
    this.searchSortAndPaginate(searchableData);
    SearchSortFilterAndPaginate.scrollToTop();
  }

  private onPreviousPagePressed(searchableData: ISearchableData): void {
    this.updatePageNumberIfPaginated(this.page - 1, searchableData);
    this.searchSortAndPaginate(searchableData);
    SearchSortFilterAndPaginate.scrollToTop();
  }

  private searchSortAndPaginate(
    searchableData: ISearchableData,
  ): void {
    const searchedElements = this.searchEnabled
      ? search(searchableData.searchableElements)
      : searchableData.searchableElements;
    const filteredElements = this.filterEnabled
      ? filterSearchableElements(searchedElements, searchableData.possibleFilters)
      : searchedElements;
    const sortedElements = sortSearchableElements(filteredElements);

    const sortedUniqueElements = _.uniqBy(sortedElements, 'parentIndex');

    const resultCount = sortedUniqueElements.length;
    SearchSortFilterAndPaginate.updateResultCount(resultCount);

    const paginatedElements = this.paginationEnabled
      ? paginateResults(sortedUniqueElements, this.page)
      : sortedUniqueElements;
    this.displaySearchableElementsAndRunPostDisplayFunction(paginatedElements);
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

  private displaySearchableElementsAndRunPostDisplayFunction(
    searchableElements: ISearchableElement[],
  ) : void {
    SearchSortFilterAndPaginate.displaySearchableElements(searchableElements);
    this.functionToRunAfterDisplayingData();
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

    if (resultCount === null) {
      return;
    }

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

  private updatePageNumberIfPaginated(
    pageNumber: number,
    searchableData: ISearchableData,
  ): void {
    if (this.paginationEnabled === false) {
      return;
    }

    this.page = pageNumber;
    this.ensurePageNumberSetInUrl(false);
    this.updateSearchableElementLinks(searchableData);
  }

  private getPageNumber(): number {
    this.ensurePageNumberSetInUrl(true);
    const currentPath = window.location.pathname;
    const urlParts = currentPath.split('/');
    return parseInt(urlParts[urlParts.length - 1], 10);
  }

  /* Guarantees the last element of the path is a number
   * with any query parameters necessary and no trailing slashes */
  private ensurePageNumberSetInUrl(preserveUrlFragment: boolean): void {
    const currentPath = window.location.pathname;
    const urlParts = currentPath.split('/');
    if (urlParts[urlParts.length - 1] === '') {
      urlParts.pop();
    }

    let currentPageNumber = parseInt(urlParts[urlParts.length - 1], 10);
    if (Number.isNaN(currentPageNumber)) {
      currentPageNumber = 1;
    } else {
      urlParts.pop();
    }

    const pageNumber = this.page ?? currentPageNumber;
    const queryParametersToRetain = this.getQueryParametersForUpdatedURL();

    const returnId = preserveUrlFragment ? window.location.hash : '';
    const newUrl = `${urlParts.join('/')}/${pageNumber}${queryParametersToRetain}${returnId}`;
    window.history.replaceState({}, '', newUrl);
  }

  private getQueryParametersForUpdatedURL(): string {
    const currentQueryParameters = window.location.search.replace('?', '');
    const separatedParameters = currentQueryParameters.split('&');
    const keptQueryParameters: string[] = [];
    separatedParameters.forEach((param) => {
      const paramName = param.split('=')[0];
      if (paramName.toUpperCase() === this.queryParameterToRetain.toUpperCase() && paramName !== '') {
        keptQueryParameters.push(param);
      }
    });

    const baseQuery = `?${SearchSortFilterAndPaginate.getBaseQueryParameters()}`;
    return keptQueryParameters.length > 0 ? `${baseQuery}&${keptQueryParameters.join('&')}` : baseQuery;
  }

  private static getBaseQueryParameters(): string {
    const searchString = getQuery();
    const sortBy = getSortBy();
    const sortDirection = getSortDirection();
    const itemsPerPage = getItemsPerPageValue().toString();
    return `searchString=${searchString}&sortBy=${sortBy}&sortDirection=${sortDirection}&itemsPerPage=${itemsPerPage}`;
  }

  private updateSearchableElementLinks(searchableData: ISearchableData): void {
    _.forEach(searchableData.searchableElements, (searchableElement) => {
      _.forEach(searchableElement.element.getElementsByTagName('a'), (anchor: HTMLAnchorElement) => {
        const shouldUpdate = anchor.getAttribute('data-return-page-enabled');
        if (shouldUpdate?.toLowerCase() === 'true') {
          const params = new URLSearchParams(anchor.search);
          const pageQueryPart = `pageNumber=${this.page.toString()}`;
          const jsScrollItemPart = `itemIdToScrollToOnReturn=${searchableElement.element.id}`;
          const returnPageQuery = `${pageQueryPart}&${SearchSortFilterAndPaginate.getBaseQueryParameters()}&${jsScrollItemPart}`;
          params.set('returnPageQuery', returnPageQuery);
          // eslint-disable-next-line no-param-reassign
          anchor.search = params.toString();
        }
      });
    });
  }

  private startLoadingSpinner(): void {
    this.spinnerContainer?.classList.remove('display-none');
    this.spinner?.classList.remove('loading-spinner');
  }

  private stopLoadingSpinner(): void {
    this.spinnerContainer?.classList.add('display-none');
    this.spinner?.classList.add('loading-spinner');
    if (this.areaToHide !== null) {
      this.areaToHide.style.display = 'inline';
    }
  }

  private static scrollToLastItemViewed(): void {
    const id = window.location.hash.split('#')[1];
    document.getElementById(id)?.scrollIntoView();
  }
}

function defaultVoidFunction(): void {
  return undefined;
}
