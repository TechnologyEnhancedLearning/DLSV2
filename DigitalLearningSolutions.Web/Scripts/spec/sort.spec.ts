/* eslint-disable @typescript-eslint/no-non-null-assertion,@typescript-eslint/ban-ts-comment */
import { JSDOM } from 'jsdom';
import { getSortValue, sortSearchableElements } from '../searchSortFilterAndPaginate/sort';
import getSearchableElements from './getSearchableElements';

describe('getSortValue', () => {
  it.each`
  fieldName             | fieldValue        | sortBy                               | expectedSortValue
  ${'name'}             | ${'Example name'} | ${'SearchableName'}                  | ${'example name'}
  ${'name'}             | ${'Example name'} | ${'Name'}                            | ${'example name'}
  ${'registration-date'}| ${'01/01/2020'}   | ${'DateRegistered'}                  | ${new Date('01/01/2020')}
  ${'started-date'}     | ${'01/01/2020'}   | ${'StartedDate'}                     | ${new Date('01/01/2020')}
  ${'accessed-date'}    | ${'02/02/2020'}   | ${'LastAccessed'}                    | ${new Date('02/02/2020')}
  ${'complete-by-date'} | ${'03/03/2020'}   | ${'CompleteByDate'}                  | ${new Date('03/03/2020')}
  ${'complete-by-date'} | ${'-'}            | ${'CompleteByDate'}                  | ${new Date(0)}
  ${'diagnostic-score'} | ${'6/10'}         | ${'HasDiagnostic,DiagnosticScore'}   | ${6}
  ${''}                 | ${''}             | ${'HasDiagnostic,DiagnosticScore'}   | ${-1}
  ${'passed-sections'}  | ${'8/10'}         | ${'IsAssessed,Passes'}               | ${8}
  ${''}                 | ${''}             | ${'IsAssessed,Passes'}               | ${-1}
  ${'brand'}            | ${'Brand 1'}      | ${'Brand'}                           | ${'brand 1'}
  ${'category'}         | ${'Category 1'}   | ${'Category'}                        | ${'category 1'}
  ${''}                 | ${''}             | ${'Category'}                        | ${''}
  ${'topic'}            | ${'Topic 1'}      | ${'Topic'}                           | ${'topic 1'}
  ${''}                 | ${''}             | ${'Topic'}                           | ${''}
  ${'delegate-count'}   | ${'5'}            | ${'DelegateCount'}                   | ${5}
  ${'courses-count'}    | ${'7'}            | ${'CoursesCount'}                    | ${7}
  ${'faq-id'}           | ${'86'}           | ${'FaqId'}                           | ${86}
  ${'faq-weighting'}    | ${'100'}          | ${'Weighting'}                       | ${100}
  `(
    'should correctly sort $fieldName by $sortBy',
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
        searchableContent: 'title',
        parentIndex: 0,
        element: document.getElementsByClassName('searchable-element')[0],
      };
      const actualValue = getSortValue(searchableElements, sortBy);

      // Then
      expect(actualValue).toEqual(expectedSortValue);
    },
  );
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
            <p name="started-date">31/01/2010</p>
            <p name="accessed-date">22/02/2010</p>
            <p name="complete-by-date">22/03/2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="searchable-element" id="course-c">
            <span name="name" class="searchable-element-title">c: Course</span>
            <p name="started-date">01/02/2010</p>
            <p name="accessed-date">22/02/2011</p>
            <p name="complete-by-date">22/03/2011</p>
            <p name="diagnostic-score">0</p>
          </div>
          <div class="searchable-element" id="course-a">
            <span name="name" class="searchable-element-title">A: course</span>
            <p name="started-date">22/01/2001</p>
            <p name="accessed-date">23/02/2011</p>
            <p name="passed-sections">0/6</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it.each`
    sortBy                               | sortDirection   | firstId       | secondId      | thirdId
    ${'Name'}                            | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
    ${'Name'}                            | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
    ${'HasDiagnostic,DiagnosticScore'}   | ${'Ascending'}  | ${'course-a'} | ${'course-c'} | ${'course-b'}
    ${'HasDiagnostic,DiagnosticScore'}   | ${'Descending'} | ${'course-b'} | ${'course-c'} | ${'course-a'}
    ${'IsAssessed,Passes'}               | ${'Ascending'}  | ${'course-c'} | ${'course-a'} | ${'course-b'}
    ${'IsAssessed,Passes'}               | ${'Descending'} | ${'course-b'} | ${'course-a'} | ${'course-c'}
    ${'StartedDate'}                     | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
    ${'StartedDate'}                     | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
    ${'LastAccessed'}                    | ${'Ascending'}  | ${'course-b'} | ${'course-c'} | ${'course-a'}
    ${'LastAccessed'}                    | ${'Descending'} | ${'course-a'} | ${'course-c'} | ${'course-b'}
    ${'CompleteByDate'}                  | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
    ${'CompleteByDate'}                  | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
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
            <p name="started-date">31/01/2010</p>
            <p name="accessed-date">22/02/2010</p>
            <p name="completed-date">22/03/2010 15:59</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="searchable-element" id="course-b">
            <span name="name" class="searchable-element-title">B: Course</span>
            <p name="started-date">01/02/2010</p>
            <p name="accessed-date">22/02/2011</p>
            <p name="completed-date">22/03/2011 10:01</p>
            <p name="diagnostic-score">0</p>
          </div>
          <div class="searchable-element" id="course-c">
            <span name="name" class="searchable-element-title">c: course</span>
            <p name="started-date">22/01/2001</p>
            <p name="accessed-date">23/02/2011</p>
            <p name="completed-date">22/03/2011 10:00</p>
            <p name="evaluated-date">24/02/2011</p>
            <p name="passed-sections">0/6</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it.each`
  sortBy         | sortDirection   | firstId       | secondId      | thirdId
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
  sortBy              | sortDirection   | firstId         | secondId        | thirdId
  ${'SearchableName'} | ${'Ascending'}  | ${'delegate-a'} | ${'delegate-b'} | ${'delegate-c'}
  ${'SearchableName'} | ${'Descending'} | ${'delegate-c'} | ${'delegate-b'} | ${'delegate-a'}
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

describe('sortSearchableElements delegates groups', () => {
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
            <p name="delegate-count">5</p>
            <p name="courses-count">7</p>
          </div>
          <div class="searchable-element" id="delegate-b">
            <span name="name" class="searchable-element-title">B: Delegate</span>
            <p name="delegate-count">1</p>
            <p name="courses-count">2</p>
          </div>
          <div class="searchable-element" id="delegate-c">
            <span name="name" class="searchable-element-title">C: Delegate</span>
            <p name="delegate-count">7</p>
            <p name="courses-count">5</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it.each`
  sortBy              | sortDirection   | firstId         | secondId        | thirdId
  ${'SearchableName'} | ${'Ascending'}  | ${'delegate-a'} | ${'delegate-b'} | ${'delegate-c'}
  ${'SearchableName'} | ${'Descending'} | ${'delegate-c'} | ${'delegate-b'} | ${'delegate-a'}
  ${'DelegateCount'}  | ${'Ascending'}  | ${'delegate-b'} | ${'delegate-a'} | ${'delegate-c'}
  ${'DelegateCount'}  | ${'Descending'} | ${'delegate-c'} | ${'delegate-a'} | ${'delegate-b'}
  ${'CoursesCount'}   | ${'Ascending'}  | ${'delegate-b'} | ${'delegate-c'} | ${'delegate-a'}
  ${'CoursesCount'}   | ${'Descending'} | ${'delegate-a'} | ${'delegate-c'} | ${'delegate-b'}
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

describe('sortSearchableElements course setup', () => {
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
            <span name="course-name" class="searchable-element-title">A: Course</span>
            <p name="delegate-count">5</p>
            <p name="in-progress-count">7</p>
          </div>
          <div class="searchable-element" id="course-b">
            <span name="course-name" class="searchable-element-title">B: Course</span>
            <p name="delegate-count">1</p>
            <p name="in-progress-count">2</p>
          </div>
          <div class="searchable-element" id="course-c">
            <span name="course-name" class="searchable-element-title">C: Course</span>
            <p name="delegate-count">7</p>
            <p name="in-progress-count">5</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it.each`
  sortBy                | sortDirection   | firstId       | secondId      | thirdId
  ${'CourseName'}       | ${'Ascending'}  | ${'course-a'} | ${'course-b'} | ${'course-c'}
  ${'CourseName'}       | ${'Descending'} | ${'course-c'} | ${'course-b'} | ${'course-a'}
  ${'DelegateCount'}    | ${'Ascending'}  | ${'course-b'} | ${'course-a'} | ${'course-c'}
  ${'DelegateCount'}    | ${'Descending'} | ${'course-c'} | ${'course-a'} | ${'course-b'}
  ${'InProgressCount'}  | ${'Ascending'}  | ${'course-b'} | ${'course-c'} | ${'course-a'}
  ${'InProgressCount'}  | ${'Descending'} | ${'course-a'} | ${'course-c'} | ${'course-b'}
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

describe('sortSearchableElements faqs', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
      <input type="text" id="select-sort-by" data-sort-by-multiple="true" />
      <input type="text" id="select-sort-direction"/>
        <div id="searchable-elements">
          <div class="searchable-element" id="faq-a">
            <span name="faq-name" class="searchable-element-title">FAQ: A</span>
            <p name="faq-weighting">55</p>
            <p name="faq-id">3</p>
          </div>
          <div class="searchable-element" id="faq-b">
            <span name="faq-name" class="searchable-element-title">FAQ: B</span>
            <p name="faq-weighting">55</p>
            <p name="faq-id">2</p>
          </div>
          <div class="searchable-element" id="faq-c">
            <span name="faq-name" class="searchable-element-title">FAQ: C</span>
            <p name="faq-weighting">99</p>
            <p name="faq-id">1</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it.each`
  sortBy         | sortDirection   | firstId    | secondId   | thirdId
  ${'Weighting'} | ${'Ascending'}  | ${'faq-a'} | ${'faq-b'} | ${'faq-c'}
  ${'Weighting'} | ${'Descending'} | ${'faq-c'} | ${'faq-a'} | ${'faq-b'}
  ${'FaqId'}     | ${'Ascending'}  | ${'faq-c'} | ${'faq-b'} | ${'faq-a'}
  ${'FaqId'}     | ${'Descending'} | ${'faq-a'} | ${'faq-b'} | ${'faq-c'}
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

  it.each`
  sortBy                | sortDirection   | firstId    | secondId   | thirdId
  ${'Weighting,FaqId'}  | ${'Ascending'}  | ${'faq-b'} | ${'faq-a'} | ${'faq-c'}
  ${'Weighting,FaqId'}  | ${'Descending'} | ${'faq-c'} | ${'faq-a'} | ${'faq-b'}
  `('should correctly sort the cards $sortDirection by multiple criteria $sortBy', ({
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
