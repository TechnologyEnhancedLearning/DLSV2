/* eslint-disable @typescript-eslint/no-non-null-assertion,
@typescript-eslint/no-explicit-any,
@typescript-eslint/ban-ts-comment */
import { JSDOM } from 'jsdom';
import { SearchSortAndPaginate } from '../learningPortal/searchSortAndPaginate';
import getCourseCards from './getCourseCards';

describe('titleFromCardElement', () => {
  it('correctly extracts the title', () => {
    // Given
    createCourseCards();

    // When
    const element = document.getElementById('course-a');
    const title = SearchSortAndPaginate.titleFromCardElement(element!);

    // Then
    expect(title).toBe('a: Course');
  });
});

describe('getCourseCards', () => {
  it('gets course cards', () => {
    // Given
    createCourseCards();
    const mockXHR = {
      open: jest.fn(),
      send: jest.fn(),
      responseXML: document,
    };
    global.XMLHttpRequest = jest.fn(() => mockXHR) as any;
    global.window = { location: { pathname: '' } } as any;

    // When
    const cardsPromise = SearchSortAndPaginate.getCourseCards('route');
    // @ts-ignore
    mockXHR.onload();
    cardsPromise.then((cards) => {
      // Then
      expect(cards?.length).toBe(2);
      expect(cards![0].title).toBe('a: Course');
    });
  });

  it('calls the correct url', () => {
    // Given
    const mockOpen = jest.fn();
    createCourseCards();
    const mockXHR = {
      open: mockOpen,
      send: jest.fn(),
      responseXML: document,
    };
    global.XMLHttpRequest = jest.fn(() => mockXHR) as any;
    global.window = { location: { pathname: '/LearningPortal/Current/1' } } as any;

    // When
    SearchSortAndPaginate.getCourseCards('test');

    // Then
    expect(mockOpen).toHaveBeenCalledWith('GET', '/LearningPortal/test', true);
  });
});

describe('displayCards', () => {
  it('correctly displays cards', () => {
    // Given
    createCourseCards();
    const cards = getCourseCards();

    // When
    document.getElementById('course-cards')!.innerHTML = '';
    SearchSortAndPaginate.displayCards(cards);

    // Then
    expect(getCourseCards().length).toBe(2);
  });
});

function createCourseCards() {
  global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <span id="page-indicator"></span>
        <div id="course-cards">
          <div class="course-card" id="course-a">
            <span name="name" class="course-title">a: Course</span>
            <p name="started-date">31-1-2010</p>
            <p name="accessed-date">22-2-2010</p>
            <p name="completed-date">22-3-2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="course-card" id="course-b">
            <span name="name" class="course-title">b: Course</span>
            <p name="started-date">31-1-2010</p>
            <p name="accessed-date">22-2-2010</p>
            <p name="completed-date">22-3-2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
        </div>
        <div class="nhsuk-pagination-item--previous"></div>
        <div class="nhsuk-pagination-item--next"></div>
      </body>
      </html>
    `).window.document;
}
