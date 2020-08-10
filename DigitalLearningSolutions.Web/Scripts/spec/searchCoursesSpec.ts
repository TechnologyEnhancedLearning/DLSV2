import { JSDOM } from 'jsdom';

// @ts-ignore
global.document = {
  getElementById: () => null,
  getElementsByClassName: () => [] as any,
};

// eslint-disable-next-line import/first
import * as searchCourses from '../learningPortal/searchCourses';

describe('titleFromCardElement', () => {
  it('should extract the correct title', () => {
    // Given
    const expectedTitle = 'example title';
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div id="current-course-cards">
          <div class="current-course-card">
            <span class="nhsuk-details__summary-text">${expectedTitle}</span>
          </div>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualTitle = searchCourses.titleFromCardElement(courseCard);

    // Then
    expect(actualTitle).toEqual(expectedTitle);
  });
});

describe('displayCards', () => {
  it('should correctly replace the contents of current-course-cards with the provided elements', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div id="current-course-cards">
          <div class="current-course-card">
            <span class="nhsuk-details__summary-text">old title</span>
          </div>
          <div class="current-course-card">
            <span class="nhsuk-details__summary-text">old title 2</span>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
    const newContents = [document.createElement('ol')];

    // When
    searchCourses.displayCards(newContents);

    // Then
    const newChildren = document.getElementById('current-course-cards')?.children;
    expect(newChildren?.length).toEqual(1);
    expect(newChildren![0].tagName).toBe('OL');
  });

  it('should display nothing when given an empty list', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div id="current-course-cards">
          <div class="current-course-card">
            <span class="nhsuk-details__summary-text">old title</span>
          </div>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    searchCourses.displayCards([]);

    // Then
    const newChildren = document.getElementById('current-course-cards')?.children;
    expect(newChildren?.length).toEqual(0);
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
    searchCourses.updateResultCount(0);

    // Then
    const resultCountElements = document.getElementById('results-count');
    expect(resultCountElements?.hidden).toBeFalse();
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
    searchCourses.updateResultCount(5);

    // Then
    const resultCountElements = document.getElementById('results-count');
    expect(resultCountElements?.textContent).toBe('5 matching results');
  });
});

describe('hideResultCount', () => {
  it('should make the result count invisible', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <span aria-hidden="false" aria-live="polite" id="results-count">0 matching results</span>
      </body>
      </html>
    `).window.document;

    // When
    searchCourses.hideResultCount();

    // Then
    const resultCountElements = document.getElementById('results-count');
    expect(resultCountElements?.hidden).toBeTrue();
    expect(resultCountElements?.getAttribute('aria-hidden')).toBe('true');
  });
});

describe('search', () => {
  it('should only show matching results', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <span hidden aria-hidden="true" aria-live="polite" id="results-count">0 matching results</span>
        <div id="current-course-cards">
          <div class="current-course-card">
            <span class="nhsuk-details__summary-text">cheese</span>
          </div>
          <div class="current-course-card">
            <span class="nhsuk-details__summary-text">petril</span>
          </div>
      </body>
      </html>
    `).window.document;

    // When
    searchCourses.search('cheese', searchCourses.getCourseCards());

    // Then
    const courseCards = document.getElementById('current-course-cards')?.children;
    expect(courseCards?.length).toBe(1);
    expect(courseCards![0].children[0].textContent).toBe('cheese');
  });
});
