/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { JSDOM } from 'jsdom';
import { SearchSortAndPaginate } from '../learningPortal/searchSortAndPaginate';

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
        </div>
        <div class="nhsuk-pagination-item--previous"></div>
        <div class="nhsuk-pagination-item--next"></div>
      </body>
      </html>
    `).window.document;
}
