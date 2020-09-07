import Details from 'nhsuk-frontend/packages/components/details/details';
import { search, setUpSearch } from './searchCourses';
import { setupSort, sortCards } from './sortCourses';
import { paginateResults, setupPagination } from './paginate';

export interface CourseCard {
  element: Element;
  title: string;
}

class SearchSortAndPaginate {
  private page: number;

  constructor() {
    this.page = 1;
    SearchSortAndPaginate.getCourseCards().then((allCards) => {
      if (allCards === undefined) {
        return;
      }

      setUpSearch(() => this.onSearchUpdated(allCards));
      setupSort(() => this.searchSortAndPaginate(allCards));
      setupPagination(
        () => this.onNextPagePressed(allCards),
        () => this.onPreviousPagePressed(allCards),
      );
    });
  }

  private onSearchUpdated(allCards: CourseCard[]) {
    this.page = 1;
    this.searchSortAndPaginate(allCards);
  }

  private onNextPagePressed(allCards: CourseCard[]) {
    this.page += 1;
    this.searchSortAndPaginate(allCards);
  }

  private onPreviousPagePressed(allCards: CourseCard[]) {
    this.page -= 1;
    this.searchSortAndPaginate(allCards);
  }

  private searchSortAndPaginate(courseCards: CourseCard[]) {
    const filteredCards = search(courseCards);
    const sortedCards = sortCards(filteredCards);
    const totalPages = Math.ceil(sortedCards.length / 10);
    const paginatedCards = paginateResults(sortedCards, this.page, totalPages);
    SearchSortAndPaginate.displayCards(paginatedCards);
  }

  static getCourseCards(): Promise<CourseCard[] | undefined> {
    return SearchSortAndPaginate.fetchAllCourseCards()
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

  static fetchAllCourseCards(): Promise<Document | null> {
    return new Promise((res) => {
      const request = new XMLHttpRequest();

      request.onload = () => {
        res(request.responseXML);
      };

      request.open('GET', '/LearningPortal/AllLearningItems', true);
      request.responseType = 'document';
      request.send();
    });
  }

  static titleFromCardElement(cardElement: Element): string {
    const titleSpan = <HTMLSpanElement>cardElement.getElementsByClassName('course-title')[0];
    return titleSpan?.textContent ?? '';
  }

  static displayCards(cards: CourseCard[]) {
    const courseCardsContainer = document.getElementById('course-cards');
    if (!courseCardsContainer) {
      return;
    }
    courseCardsContainer.textContent = '';
    cards.forEach((card) => courseCardsContainer.appendChild(card.element));
    // This is required to polyfill the new elements in IE
    Details();
  }
}

// eslint-disable-next-line no-new
new SearchSortAndPaginate();
