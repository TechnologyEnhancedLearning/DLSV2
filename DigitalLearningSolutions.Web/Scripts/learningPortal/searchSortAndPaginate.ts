import Details from 'nhsuk-frontend/packages/components/details/details';
import { search, setUpSearch } from './searchCourses';
import { setupSort, sortCards } from './sortCourses';
import { ITEMS_PER_PAGE, paginateResults, setupPagination } from './paginate';

export interface CourseCard {
  element: Element;
  title: string;
}

export class SearchSortAndPaginate {
  private page: number;

  constructor(route: string) {
    this.page = 1;
    SearchSortAndPaginate.getCourseCards(route).then((allCards) => {
      if (allCards === undefined) {
        return;
      }

      setUpSearch(() => this.onSearchUpdated(allCards));
      setupSort(() => this.searchSortAndPaginate(allCards));
      setupPagination(
        () => this.onNextPagePressed(allCards),
        () => this.onPreviousPagePressed(allCards),
      );
      this.searchSortAndPaginate(allCards);
    });
  }

  private onSearchUpdated(allCards: CourseCard[]): void {
    this.page = 1;
    this.searchSortAndPaginate(allCards);
  }

  private onNextPagePressed(allCards: CourseCard[]): void {
    this.page += 1;
    this.searchSortAndPaginate(allCards);
  }

  private onPreviousPagePressed(allCards: CourseCard[]): void {
    this.page -= 1;
    this.searchSortAndPaginate(allCards);
  }

  private searchSortAndPaginate(courseCards: CourseCard[]): void {
    const filteredCards = search(courseCards);
    const sortedCards = sortCards(filteredCards);
    const totalPages = Math.ceil(sortedCards.length / ITEMS_PER_PAGE);
    const paginatedCards = paginateResults(sortedCards, this.page, totalPages);
    SearchSortAndPaginate.displayCards(paginatedCards);
  }

  static getCourseCards(route: string): Promise<CourseCard[] | undefined> {
    return SearchSortAndPaginate.fetchAllCourseCards(route)
      .then((response): CourseCard[] | undefined => {
        if (response === null) {
          return undefined;
        }

        const courseCardElements = Array.from(response.getElementsByClassName('course-card'));
        return courseCardElements.map((element) => ({
          element,
          title: SearchSortAndPaginate.titleFromCardElement(element),
        }));
      });
  }

  private static fetchAllCourseCards(route: string): Promise<Document | null> {
    const path = this.getPathForEndpoint(route);
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

  static titleFromCardElement(cardElement: Element): string {
    const titleSpan = <HTMLSpanElement>cardElement.getElementsByClassName('course-title')[0];
    return titleSpan?.textContent ?? '';
  }

  static displayCards(cards: CourseCard[]): void {
    const courseCardsContainer = document.getElementById('course-cards');
    if (!courseCardsContainer) {
      return;
    }
    courseCardsContainer.textContent = '';
    cards.forEach((card) => courseCardsContainer.appendChild(card.element));
    // This is required to polyfill the new elements in IE
    Details();
  }

  private static getPathForEndpoint(endpoint: string): string {
    const currentPath = window.location.pathname;
    const indexOfBaseUrl = currentPath.indexOf('LearningPortal');
    return `${currentPath.substring(0, indexOfBaseUrl)}LearningPortal/${endpoint}`;
  }
}
