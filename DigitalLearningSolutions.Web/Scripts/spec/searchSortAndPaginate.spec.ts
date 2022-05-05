/* eslint-disable @typescript-eslint/no-non-null-assertion,
@typescript-eslint/no-explicit-any,
@typescript-eslint/ban-ts-comment */
import { JSDOM } from 'jsdom';
import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import getSearchableElements from './getSearchableElements';

describe('titleFromCardElement', () => {
  it('correctly extracts the title', () => {
    // Given
    createCourseCards();

    // When
    const element = document.getElementById('course-a');
    const title = SearchSortFilterAndPaginate.searchableContentFromElement(element!, 'title');

    // Then
    expect(title).toBe('a: Course');
  });
});

describe('descriptionFromCardElement', () => {
  it('correctly extracts the description', () => {
    // Given
    createCourseCards();

    // When
    const element = document.getElementById('course-b');
    const description = SearchSortFilterAndPaginate.searchableContentFromElement(element!, 'description');

    // Then
    expect(description).toBe('A course for b');
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
    const cardsPromise = SearchSortFilterAndPaginate.getSearchableElements('route', ['title']);
    // @ts-ignore
    mockXHR.onload();
    cardsPromise.then((cards) => {
      // Then
      expect(cards?.searchableElements.length).toBe(2);
      expect(cards!.searchableElements[0].searchableContent).toBe('a: Course');
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
    SearchSortFilterAndPaginate.getSearchableElements('LearningPortal/test', ['title']);

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
    SearchSortFilterAndPaginate.getSearchableElements('LearningPortal/test', ['title']);

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
    SearchSortFilterAndPaginate.displaySearchableElements(cards);

    // Then
    expect(getSearchableElements().length).toBe(2);
  });
});

describe('updateResultCount', () => {
  it('should make the result count visible', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <span hidden aria-hidden="true" aria-live="polite" id="results-count">0 matching results</span>
      </body>
      </html>
    `).window.document;

    // When
    SearchSortFilterAndPaginate.updateResultCount(0);

    // Then
    const resultCountElements = document.getElementById('results-count');
    expect(resultCountElements?.hidden).toBeFalsy();
    expect(resultCountElements?.getAttribute('aria-hidden')).toBe('false');
  });

  it('should show the correct result count', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <span hidden aria-hidden="true" aria-live="polite" id="results-count">0 matching results</span>
      </body>
      </html>
    `).window.document;

    // When
    SearchSortFilterAndPaginate.updateResultCount(5);

    // Then
    const resultCountElements = document.getElementById('results-count');
    expect(resultCountElements?.textContent).toBe('5 matching results');
  });
});

function createCourseCards() {
  global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <span class="page-indicator"></span>
        <div id="searchable-elements">
          <div class="searchable-element" id="course-a">
            <span name="name" class="searchable-element-title">a: Course</span>
            <span name="description" class="searchable-element-description">A course for a</span>
            <p name="started-date">31-1-2010</p>
            <p name="accessed-date">22-2-2010</p>
            <p name="completed-date">22-3-2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="searchable-element" id="course-b">
            <span name="name" class="searchable-element-title">b: Course</span>
            <span name="description" class="searchable-element-description">A course for b</span>
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
