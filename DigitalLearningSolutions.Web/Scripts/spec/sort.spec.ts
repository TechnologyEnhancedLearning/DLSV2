/* eslint-disable @typescript-eslint/no-non-null-assertion,@typescript-eslint/ban-ts-comment */
import { JSDOM } from 'jsdom';
import { getSortValue, sortSearchableElements } from '../searchSortFilterAndPaginate/sort';
import getSearchableElements from './getSearchableElements';

describe('getSortValue', () => {
  it.each`
  fieldName             | fieldValue        | sortBy                  | expectedSortValue
  ${'name'}             | ${'Example name'} | ${'SearchableName'}        | ${'example name'}
  ${'name'}             | ${'Example name'} | ${'Name'}        | ${'example name'}
  ${'registration-date'}| ${'01/01/2020'}   | ${'DateRegistered'}      | ${new Date('01/01/2020')}
  ${'started-date'}     | ${'01/01/2020'}   | ${'StartedDate'}      | ${new Date('01/01/2020')}
  ${'accessed-date'}    | ${'02/02/2020'}   | ${'LastAccessed'} | ${new Date('02/02/2020')}
  ${'complete-by-date'} | ${'03/03/2020'}   | ${'CompleteByDate'}   | ${new Date('03/03/2020')}
  ${'complete-by-date'} | ${'-'}            | ${'CompleteByDate'}   | ${new Date(0)}
  ${'diagnostic-score'} | ${'6/10'}         | ${'HasDiagnostic,DiagnosticScore'}   | ${6}
  ${''}                 | ${''}             | ${'HasDiagnostic,DiagnosticScore'}   | ${-1}
  ${'passed-sections'}  | ${'8/10'}         | ${'IsAssessed,Passes'}    | ${8}
  ${''}                 | ${''}             | ${'IsAssessed,Passes'}    | ${-1}
  ${'brand'}            | ${'Brand 1'}      | ${'Brand'}              | ${'brand 1'}
  ${'category'}         | ${'Category 1'}   | ${'Category'}           | ${'category 1'}
  ${''}                 | ${''}             | ${'Category'}           | ${''}
  ${'topic'}            | ${'Topic 1'}      | ${'Topic'}              | ${'topic 1'}
  ${''}                 | ${''}             | ${'Topic'}              | ${''}
  `('should correctly sort $fieldName by $sortBy',
    ({
      fieldName, fieldValue, sortBy, expectedSortValue,
    }) => {
      // Given
      global.document = new JSDOM(`
        <html>
        <head></head>
        <body>
          <div class="searchable-element">
            <span name="${fieldName}">${fieldValue}</span>
          </div>
        </body>
        </html>
      `).window.document;

      // When
      const searchableElements = {
        title: ' title',
        element: document.getElementsByClassName('searchable-element')[0],
      };
      const actualValue = getSortValue(searchableElements, sortBy);

      // Then
      expect(actualValue).toEqual(expectedSortValue);
    });
});

describe('sortSearchableElements current', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
      <input type="text" id="select-sort-by" />
      <input type="text" id="select-sort-direction"/>
        <div id="searchable-elements">
          <div class="searchable-element" id="course-b">
            <span name="name" class="searchable-element-title">B: Course</span>
            <p name="started-date">31-1-2010</p>
            <p name="accessed-date">22-2-2010</p>
            <p name="complete-by-date">22-3-2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="searchable-element" id="course-c">
            <span name="name" class="searchable-element-title">c: Course</span>
            <p name="started-date">1-2-2010</p>
            <p name="accessed-date">22-2-2011</p>
            <p name="complete-by-date">22-3-2011</p>
            <p name="diagnostic-score">0</p>
          </div>
          <div class="searchable-element" id="course-a">
            <span name="name" class="searchable-element-title">A: course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it.each`
    sortBy                  | sortDirection   | firstId       | secondId      | thirdId
    ${'Name'}        | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
    ${'Name'}        | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
    ${'HasDiagnostic,DiagnosticScore'}   | ${'Ascending'}  | ${'course-a'} | ${'course-c'} | ${'course-b'}
    ${'HasDiagnostic,DiagnosticScore'}   | ${'Descending'} | ${'course-b'} | ${'course-c'} | ${'course-a'}
    ${'IsAssessed,Passes'}    | ${'Ascending'}  | ${'course-c'} | ${'course-a'} | ${'course-b'}
    ${'IsAssessed,Passes'}    | ${'Descending'} | ${'course-b'} | ${'course-a'} | ${'course-c'}
    ${'StartedDate'}      | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
    ${'StartedDate'}      | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
    ${'LastAccessed'} | ${'Ascending'}  | ${'course-b'} | ${'course-c'} | ${'course-a'}
    ${'LastAccessed'} | ${'Descending'} | ${'course-a'} | ${'course-c'} | ${'course-b'}
    ${'CompleteByDate'}   | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
    ${'CompleteByDate'}   | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
    `('should correctly sort the cards $sortDirection by $sortBy', ({
    sortBy, sortDirection, firstId, secondId, thirdId,
  }) => {
    // When
    setSortBy(sortBy);
    setSortDirection(sortDirection);
    const searchableElements = getSearchableElements();
    const newSearchableElements = sortSearchableElements(searchableElements);

    // Then
    expect(newSearchableElements?.length).toBe(3);
    expect(newSearchableElements![0].element.id).toBe(firstId);
    expect(newSearchableElements![1].element.id).toBe(secondId);
    expect(newSearchableElements![2].element.id).toBe(thirdId);
  });
});

describe('sortSearchableElements completed', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
      <input type="text" id="select-sort-by" />
      <input type="text" id="select-sort-direction"/>
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
        </div>
      </body>
      </html>
    `).window.document;
  });

  it.each`
  sortBy              | sortDirection   | firstId       | secondId      | thirdId
  ${'Completed'} | ${'Ascending'}  | ${'course-a'} | ${'course-c'} | ${'course-b'}
  ${'Completed'} | ${'Descending'} | ${'course-b'} | ${'course-c'} | ${'course-a'}
  `('should correctly sort the cards $sortDirection by $sortBy', ({
    sortBy, sortDirection, firstId, secondId, thirdId,
  }) => {
    // When
    setSortBy(sortBy);
    setSortDirection(sortDirection);
    const searchableElements = getSearchableElements();
    const newSearchableElements = sortSearchableElements(searchableElements);

    // Then
    expect(newSearchableElements?.length).toEqual(3);
    expect(newSearchableElements![0].element.id).toBe(firstId);
    expect(newSearchableElements![1].element.id).toBe(secondId);
    expect(newSearchableElements![2].element.id).toBe(thirdId);
  });
});

describe('sortSearchableElements available', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
      <input type="text" id="select-sort-by" />
      <input type="text" id="select-sort-direction"/>
        <div id="searchable-elements">
          <div class="searchable-element" id="course-a">
            <span name="name" class="searchable-element-title">A: Course</span>
            <p name="brand">C: Brand</p>
            <p name="topic">Topic 2</p>
          </div>
          <div class="searchable-element" id="course-b">
            <span name="name" class="searchable-element-title">B: Course</span>
            <p name="brand">A: Brand</p>
            <p name="category">C: Category</p>
            <p name="topic">Topic 1</p>
          </div>
          <div class="searchable-element" id="course-c">
            <span name="name" class="searchable-element-title">C: Course</span>
            <p name="brand">B: Brand</p>
            <p name="category">B: Category</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it.each`
  sortBy        | sortDirection   | firstId       | secondId      | thirdId
  ${'Brand'}    | ${'Ascending'}  | ${'course-b'} | ${'course-c'} | ${'course-a'}
  ${'Brand'}    | ${'Descending'} | ${'course-a'} | ${'course-c'} | ${'course-b'}
  ${'Category'} | ${'Ascending'}  | ${'course-a'} | ${'course-c'} | ${'course-b'}
  ${'Category'} | ${'Descending'} | ${'course-b'} | ${'course-c'} | ${'course-a'}
  ${'Topic'}    | ${'Ascending'}  | ${'course-c'} | ${'course-b'} | ${'course-a'}
  ${'Topic'}    | ${'Descending'} | ${'course-a'} | ${'course-b'} | ${'course-c'}
  `('should correctly sort the cards $sortDirection by $sortBy', ({
    sortBy, sortDirection, firstId, secondId, thirdId,
  }) => {
    // When
    setSortBy(sortBy);
    setSortDirection(sortDirection);
    const searchableElements = getSearchableElements();
    const newSearchableElements = sortSearchableElements(searchableElements);

    // Then
    expect(newSearchableElements?.length).toEqual(3);
    expect(newSearchableElements![0].element.id).toBe(firstId);
    expect(newSearchableElements![1].element.id).toBe(secondId);
    expect(newSearchableElements![2].element.id).toBe(thirdId);
  });
});

describe('sortSearchableElements delegates', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
      <input type="text" id="select-sort-by" />
      <input type="text" id="select-sort-direction"/>
        <div id="searchable-elements">
          <div class="searchable-element" id="delegate-a">
            <span name="name" class="searchable-element-title">A: Delegate</span>
            <p name="registration-date">02/02/2021</p>
          </div>
          <div class="searchable-element" id="delegate-b">
            <span name="name" class="searchable-element-title">B: Delegate</span>
            <p name="registration-date">01/01/2021</p>
          </div>
          <div class="searchable-element" id="delegate-c">
            <span name="name" class="searchable-element-title">C: Delegate</span>
            <p name="registration-date">03/03/2021</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it.each`
  sortBy                | sortDirection   | firstId       | secondId      | thirdId
  ${'SearchableName'}   | ${'Ascending'}  | ${'delegate-a'} | ${'delegate-b'} | ${'delegate-c'}
  ${'SearchableName'}   | ${'Descending'} | ${'delegate-c'} | ${'delegate-b'} | ${'delegate-a'}
  ${'DateRegistered'} | ${'Ascending'}  | ${'delegate-b'} | ${'delegate-a'} | ${'delegate-c'}
  ${'DateRegistered'} | ${'Descending'} | ${'delegate-c'} | ${'delegate-a'} | ${'delegate-b'}
  `('should correctly sort the cards $sortDirection by $sortBy', ({
    sortBy, sortDirection, firstId, secondId, thirdId,
  }) => {
    // When
    setSortBy(sortBy);
    setSortDirection(sortDirection);
    const searchableElements = getSearchableElements();
    const newSearchableElements = sortSearchableElements(searchableElements);

    // Then
    expect(newSearchableElements?.length).toEqual(3);
    expect(newSearchableElements![0].element.id).toBe(firstId);
    expect(newSearchableElements![1].element.id).toBe(secondId);
    expect(newSearchableElements![2].element.id).toBe(thirdId);
  });
});

function setSortBy(sortBy: string) {
  (<HTMLInputElement>document.getElementById('select-sort-by')).value = sortBy;
}

function setSortDirection(sortDirection: string) {
  (<HTMLInputElement>document.getElementById('select-sort-direction')).value = sortDirection;
}
