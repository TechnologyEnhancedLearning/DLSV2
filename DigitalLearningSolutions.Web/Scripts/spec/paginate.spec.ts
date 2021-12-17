/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { JSDOM } from 'jsdom';
import * as paginate from '../searchSortFilterAndPaginate/paginate';
import getSearchableElements from './getSearchableElements';

describe('paginateResults', () => {
  it('returns ten results per page', () => {
    // Given
    const courseCards = createCourseCards();

    // When
    const paginatedCards = paginate.paginateResults(courseCards, 1);

    // Then
    expect(paginatedCards.length).toBe(10);
  });

  it('returns the second page of results', () => {
    // Given
    const courseCards = createCourseCards();

    // When
    const paginatedCards = paginate.paginateResults(courseCards, 2);

    // Then
    expect(paginatedCards.length).toBe(1);
    expect(paginatedCards[0].title).toBe('k: course');
  });

  it('updates the page number', () => {
    // Given
    const courseCards = createCourseCards();

    // When
    paginate.paginateResults(courseCards, 1);

    // Then
    const pageText = document.getElementById('page-indicator')!.textContent;
    expect(pageText).toBe('1 of 2');
    const pageIndicator = document.getElementById('page-indicator');
    expect(pageIndicator!.hidden).toBeFalsy();
  });

  it('hides the page indicator when there is only one page', () => {
    // Given
    const courseCards = createCourseCards();

    (<HTMLSelectElement>document.getElementById('items-per-page-select')).value = '11';

    // When
    paginate.paginateResults(courseCards, 1);

    // Then
    const pageIndicator = document.getElementById('page-indicator');
    expect(pageIndicator!.hidden).toBeTruthy();
  });

  it('hides the previous button when on the first page', () => {
    // Given
    const courseCards = createCourseCards();

    // When
    paginate.paginateResults(courseCards, 1);

    // Then
    const previousButton = <HTMLElement>document.getElementsByClassName('nhsuk-pagination-item--previous')[0];
    expect(previousButton.hidden).toBeTruthy();
  });

  it('shows the next button when there are more pages', () => {
    // Given
    const courseCards = createCourseCards();

    // When
    paginate.paginateResults(courseCards, 1);

    // Then
    const nextButton = <HTMLElement>document.getElementsByClassName('nhsuk-pagination-item--next')[0];
    expect(nextButton.hidden).toBeFalsy();
  });

  it('hides the next button when on the last page', () => {
    // Given
    const courseCards = createCourseCards();

    // When
    paginate.paginateResults(courseCards, 2);

    // Then
    const nextButton = <HTMLElement>document.getElementsByClassName('nhsuk-pagination-item--next')[0];
    expect(nextButton.hidden).toBeTruthy();
  });

  it('shows the previous button when not on the first page', () => {
    // Given
    const courseCards = createCourseCards();

    // When
    paginate.paginateResults(courseCards, 2);

    // Then
    const previousButton = <HTMLElement>document.getElementsByClassName('nhsuk-pagination-item--previous')[0];
    expect(previousButton.hidden).toBeFalsy();
  });

  it('shows 11 results when 11 items per page is selected', () => {
    // Given
    const courseCards = createCourseCards();

    (<HTMLSelectElement>document.getElementById('items-per-page-select')).value = '11';

    // When
    const paginatedCards = paginate.paginateResults(courseCards, 1);

    // Then
    expect(paginatedCards.length).toBe(11);
  });
});

function createCourseCards() {
  global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <select id="items-per-page-select">
          <option value="10" selected>10</option>
          <option value="11">11</option>
        </select>
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
            <span name="name" class="searchable-element-title">B: Course</span>
            <p name="started-date">1-2-2010</p>
            <p name="accessed-date">22-2-2011</p>
            <p name="completed-date">22-3-2011</p>
            <p name="diagnostic-score">0</p>
          </div>
          <div class="searchable-element" id="course-c">
            <span name="name" class="searchable-element-title">c: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="completed-date">22-2-2011</p>
            <p name="evaluated-date">24-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
          <div class="searchable-element" id="course-d">
            <span name="name" class="searchable-element-title">d: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="completed-date">22-2-2011</p>
            <p name="evaluated-date">24-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
          <div class="searchable-element" id="course-e">
            <span name="name" class="searchable-element-title">e: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="completed-date">22-2-2011</p>
            <p name="evaluated-date">24-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
          <div class="searchable-element" id="course-f">
            <span name="name" class="searchable-element-title">f: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="completed-date">22-2-2011</p>
            <p name="evaluated-date">24-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
          <div class="searchable-element" id="course-g">
            <span name="name" class="searchable-element-title">g: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="completed-date">22-2-2011</p>
            <p name="evaluated-date">24-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
          <div class="searchable-element" id="course-h">
            <span name="name" class="searchable-element-title">h: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="completed-date">22-2-2011</p>
            <p name="evaluated-date">24-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
          <div class="searchable-element" id="course-i">
            <span name="name" class="searchable-element-title">i: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="completed-date">22-2-2011</p>
            <p name="evaluated-date">24-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
          <div class="searchable-element" id="course-j">
            <span name="name" class="searchable-element-title">j: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="completed-date">22-2-2011</p>
            <p name="evaluated-date">24-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
          <div class="searchable-element" id="course-k">
            <span name="name" class="searchable-element-title">k: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="completed-date">22-2-2011</p>
            <p name="evaluated-date">24-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
        </div>
<div class="nhsuk-pagination" role="navigation" aria-label="Pagination">
        <div class="nhsuk-pagination-item--previous"></div>
        <div class="nhsuk-pagination-item--next"></div>
</div>
      </body>
      </html>
    `).window.document;
  return getSearchableElements();
}
