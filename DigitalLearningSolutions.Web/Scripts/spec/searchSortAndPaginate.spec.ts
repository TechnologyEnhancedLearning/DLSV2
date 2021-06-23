/* eslint-disable @typescript-eslint/no-non-null-assertion,
@typescript-eslint/no-explicit-any,
@typescript-eslint/ban-ts-comment */
import { JSDOM } from 'jsdom';
import { SearchSortAndPaginate } from '../searchSortAndPaginate/searchSortAndPaginate';
import getSearchableElements from './getSearchableElements';

describe('titleFromCardElement', () => {
  it('correctly extracts the title', () => {
    // Given
    createCourseCards();

    // When
    const element = document.getElementById('course-a');
    const title = SearchSortAndPaginate.titleFromElement(element!);

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
    const cardsPromise = SearchSortAndPaginate.getSearchableElements('route');
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
    SearchSortAndPaginate.getSearchableElements('/LearningPortal/test');

    // Then
    expect(mockOpen).toHaveBeenCalledWith('GET', '/LearningPortal/test', true);
  });

  it('calls the correct url hosted in subdirectory', () => {
    // Given
    const mockOpen = jest.fn();
    createCourseCards();
    const mockXHR = {
      open: mockOpen,
      send: jest.fn(),
      responseXML: document,
    };
    global.XMLHttpRequest = jest.fn(() => mockXHR) as any;
    global.window = { location: { pathname: '/dev/LearningPortal/Current/1' } } as any;

    // When
    SearchSortAndPaginate.getSearchableElements('/LearningPortal/test');

    // Then
    expect(mockOpen).toHaveBeenCalledWith('GET', '/dev/LearningPortal/test', true);
  });
});

describe('displayCards', () => {
  it('correctly displays cards', () => {
    // Given
    createCourseCards();
    const cards = getSearchableElements();

    // When
    document.getElementById('searchable-elements')!.innerHTML = '';
    SearchSortAndPaginate.displaySearchableElements(cards);

    // Then
    expect(getSearchableElements().length).toBe(2);
  });
});

function createCourseCards() {
  global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <span id="page-indicator"></span>
        <div id="searchable-elements">
          <div class="searchable-element" id="course-a">
            <span name="name" class="searchable-element-title">a: Course</span>
            <p name="started-date">31-1-2010</p>
            <p name="accessed-date">22-2-2010</p>
            <p name="completed-date">22-3-2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="searchable-element" id="course-b">
            <span name="name" class="searchable-element-title">b: Course</span>
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
