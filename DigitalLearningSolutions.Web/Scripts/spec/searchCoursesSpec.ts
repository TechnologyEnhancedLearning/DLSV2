import { JSDOM } from 'jsdom';

// @ts-ignore
global.document = {
  getElementById: () => null,
  getElementsByClassName: () => [] as any,
};

// eslint-disable-next-line import/first
import * as searchCourses from '../learningPortal/searchCourses';
// eslint-disable-next-line import/first
import * as sortCourses from '../learningPortal/sortCourses';

describe('titleFromCardElement', () => {
  it('should extract the correct title', () => {
    // Given
    const expectedTitle = 'example title';
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div id="course-cards">
          <div class="course-card">
            <span class="nhsuk-details__summary-text course-title">${expectedTitle}</span>
          </div>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('course-card')[0];
    const actualTitle = searchCourses.titleFromCardElement(courseCard);

    // Then
    expect(actualTitle).toEqual(expectedTitle);
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
    const displaySpy = spyOn(sortCourses, 'sortAndDisplaySearchResults').and.stub();
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <span hidden aria-hidden="true" aria-live="polite" id="results-count">0 matching results</span>
        <div id="course-cards">
          <div class="course-card">
            <span class="nhsuk-details__summary-text course-title">cheese</span>
          </div>
          <div class="course-card">
            <span class="nhsuk-details__summary-text">petril</span>
          </div>
      </body>
      </html>
    `).window.document;

    // When
    searchCourses.search('cheese', searchCourses.getCourseCards());

    // Then
    expect(displaySpy).toHaveBeenCalledTimes(1);
    expect(displaySpy.calls.mostRecent().args[0].length).toBe(1);
  });
});
