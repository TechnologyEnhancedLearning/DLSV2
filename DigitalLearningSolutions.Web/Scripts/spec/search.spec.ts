/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { JSDOM } from 'jsdom';
import * as searchCourses from '../searchSortFilterAndPaginate/search';
import getSearchableElements from './getSearchableElements';

describe('search', () => {
  it('should only show matching results', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <input type="text" id="search-field" value="cheese" />
        <span hidden aria-hidden="true" aria-live="polite" class="results-count">0 matching results</span>
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
    expect(newCourses[0].searchableContent).toBe('cheese');
  });
});
