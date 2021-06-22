/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { JSDOM } from 'jsdom';
import * as searchCourses from '../searchSortAndPaginate/search';
import getSearchableElements from './getSearchableElements';

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
    expect(resultCountElements?.hidden).toBeTruthy();
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
        <input type="text" id="search-field" value="cheese" />
        <span hidden aria-hidden="true" aria-live="polite" id="results-count">0 matching results</span>
        <div id="searchable-elements">
          <div class="searchable-element">
            <span class="nhsuk-details__summary-text searchable-element-title">cheese</span>
          </div>
          <div class="searchable-element">
            <span class="nhsuk-details__summary-text searchable-element-title">petril</span>
          </div>
      </body>
      </html>
    `).window.document;

    // When
    const newCourses = searchCourses.search(getSearchableElements());

    // Then
    expect(newCourses.length).toBe(1);
    expect(newCourses[0].title).toBe('cheese');
  });
});
