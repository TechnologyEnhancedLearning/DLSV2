/* eslint-disable @typescript-eslint/no-non-null-assertion,@typescript-eslint/ban-ts-comment */
import { JSDOM } from 'jsdom';
import { getSortValue, sortCards } from '../learningPortal/sortCourses';
import getCourseCards from './getCourseCards';

describe('getSortValue', () => {
  it.each`
  fieldName             | fieldValue        | sortBy                  | expectedSortValue
  ${'name'}             | ${'Example name'} | ${'Course Name'}        | ${'example name'}
  ${'started-date'}     | ${'01/01/2020'}   | ${'Enrolled Date'}      | ${new Date('01/01/2020')}
  ${'accessed-date'}    | ${'02/02/2020'}   | ${'Last Accessed Date'} | ${new Date('02/02/2020')}
  ${'complete-by-date'} | ${'03/03/2020'}   | ${'Complete By Date'}   | ${new Date('03/03/2020')}
  ${'complete-by-date'} | ${'-'}            | ${'Complete By Date'}   | ${new Date(0)}
  ${'diagnostic-score'} | ${'6/10'}         | ${'Diagnostic Score'}   | ${6}
  ${''}                 | ${''}             | ${'Diagnostic Score'}   | ${-1}
  ${'passed-sections'}  | ${'8/10'}         | ${'Passed Sections'}    | ${8}
  ${''}                 | ${''}             | ${'Passed Sections'}    | ${-1}
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
          <div class="course-card">
            <span name="${fieldName}">${fieldValue}</span>
          </div>
        </body>
        </html>
      `).window.document;

      // When
      const courseCard = {
        title: ' title',
        element: document.getElementsByClassName('course-card')[0],
      };
      const actualValue = getSortValue(courseCard, sortBy);

      // Then
      expect(actualValue).toEqual(expectedSortValue);
    });
});

describe('sortCards current', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
      <input type="text" id="select-sort-by" />
      <input type="text" id="select-sort-direction"/>
        <div id="course-cards">
          <div class="course-card" id="course-b">
            <span name="name" class="course-title">B: Course</span>
            <p name="started-date">31-1-2010</p>
            <p name="accessed-date">22-2-2010</p>
            <p name="complete-by-date">22-3-2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="course-card" id="course-c">
            <span name="name" class="course-title">c: Course</span>
            <p name="started-date">1-2-2010</p>
            <p name="accessed-date">22-2-2011</p>
            <p name="complete-by-date">22-3-2011</p>
            <p name="diagnostic-score">0</p>
          </div>
          <div class="course-card" id="course-a">
            <span name="name" class="course-title">A: course</span>
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
    ${'Course Name'}        | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
    ${'Course Name'}        | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
    ${'Diagnostic Score'}   | ${'Ascending'}  | ${'course-a'} | ${'course-c'} | ${'course-b'}
    ${'Diagnostic Score'}   | ${'Descending'} | ${'course-b'} | ${'course-c'} | ${'course-a'}
    ${'Passed Sections'}    | ${'Ascending'}  | ${'course-c'} | ${'course-a'} | ${'course-b'}
    ${'Passed Sections'}    | ${'Descending'} | ${'course-b'} | ${'course-a'} | ${'course-c'}
    ${'Enrolled Date'}      | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
    ${'Enrolled Date'}      | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
    ${'Last Accessed Date'} | ${'Ascending'}  | ${'course-b'} | ${'course-c'} | ${'course-a'}
    ${'Last Accessed Date'} | ${'Descending'} | ${'course-a'} | ${'course-c'} | ${'course-b'}
    ${'Complete By Date'}   | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
    ${'Complete By Date'}   | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
    `('should correctly sort the cards $sortDirection by $sortBy', ({
    sortBy, sortDirection, firstId, secondId, thirdId,
  }) => {
    // When
    setSortBy(sortBy);
    setSortDirection(sortDirection);
    const courseCards = getCourseCards();
    const newCards = sortCards(courseCards);

    // Then
    expect(newCards?.length).toBe(3);
    expect(newCards![0].element.id).toBe(firstId);
    expect(newCards![1].element.id).toBe(secondId);
    expect(newCards![2].element.id).toBe(thirdId);
  });
});

describe('sortCards completed', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
      <input type="text" id="select-sort-by" />
      <input type="text" id="select-sort-direction"/>
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
            <span name="name" class="course-title">B: Course</span>
            <p name="started-date">1-2-2010</p>
            <p name="accessed-date">22-2-2011</p>
            <p name="completed-date">22-3-2011</p>
            <p name="diagnostic-score">0</p>
          </div>
          <div class="course-card" id="course-c">
            <span name="name" class="course-title">c: course</span>
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
  ${'Completed Date'} | ${'Ascending'}  | ${'course-a'} | ${'course-c'} | ${'course-b'}
  ${'Completed Date'} | ${'Descending'} | ${'course-b'} | ${'course-c'} | ${'course-a'}
  `('should correctly sort the cards $sortDirection by $sortBy', ({
    sortBy, sortDirection, firstId, secondId, thirdId,
  }) => {
    // When
    setSortBy(sortBy);
    setSortDirection(sortDirection);
    const courseCards = getCourseCards();
    const newCards = sortCards(courseCards);

    // Then
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].element.id).toBe(firstId);
    expect(newCards![1].element.id).toBe(secondId);
    expect(newCards![2].element.id).toBe(thirdId);
  });
});

describe('sortCards available', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
      <input type="text" id="select-sort-by" />
      <input type="text" id="select-sort-direction"/>
        <div id="course-cards">
          <div class="course-card" id="course-a">
            <span name="name" class="course-title">A: Course</span>
            <p name="brand">C: Brand</p>
            <p name="topic">Topic 2</p>
          </div>
          <div class="course-card" id="course-b">
            <span name="name" class="course-title">B: Course</span>
            <p name="brand">A: Brand</p>
            <p name="category">C: Category</p>
            <p name="topic">Topic 1</p>
          </div>
          <div class="course-card" id="course-c">
            <span name="name" class="course-title">C: Course</span>
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
    const courseCards = getCourseCards();
    const newCards = sortCards(courseCards);

    // Then
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].element.id).toBe(firstId);
    expect(newCards![1].element.id).toBe(secondId);
    expect(newCards![2].element.id).toBe(thirdId);
  });
});

function setSortBy(sortBy: string) {
  (<HTMLInputElement>document.getElementById('select-sort-by')).value = sortBy;
}

function setSortDirection(sortDirection: string) {
  (<HTMLInputElement>document.getElementById('select-sort-direction')).value = sortDirection;
}
