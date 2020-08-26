import { JSDOM } from 'jsdom';
import { cases } from 'jasmine-parameterized';

// @ts-ignore
global.document = {
  getElementById: () => null,
  getElementsByClassName: () => [] as any,
};

// eslint-disable-next-line import/first
import { sortCards, getSortValue } from '../learningPortal/sortCourses';

describe('getSortValue', () => {
  cases([
    ['name', 'example name', 'Course Name', 'example name'],
    ['started-date', '01/01/2020', 'Enrolled Date', new Date('01/01/2020')],
    ['accessed-date', '02/02/2020', 'Last Accessed Date', new Date('02/02/2020')],
    ['complete-by-date', '03/03/2020', 'Complete By Date', new Date('03/03/2020')],
    ['complete-by-date', '-', 'Complete By Date', new Date(0)],
    ['diagnostic-score', '6/10', 'Diagnostic Score', 6],
    ['', '', 'Diagnostic Score', -1],
    ['passed-sections', '8/10', 'Passed Sections', 8],
    ['', '', 'Passed Sections', -1],
    ['brand', 'Brand 1', 'Brand', 'Brand 1'],
    ['category', 'Category 1', 'Category', 'Category 1'],
    ['', '', 'Category', ''],
    ['topic', 'Topic 1', 'Topic', 'Topic 1'],
    ['', '', 'Topic', ''],
  ])
    .it('should correctly extract sort by fields', ([fieldName, fieldValue, sortBy, expectedSortValue]) => {
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
      const courseCard = document.getElementsByClassName('course-card')[0];
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
        <div id="course-cards">
          <div class="course-card" id="course-b"> 
            <span name="name">B: Course</span>
            <p name="started-date">31-1-2010</p>
            <p name="accessed-date">22-2-2010</p>
            <p name="complete-by-date">22-3-2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="course-card" id="course-c"> 
            <span name="name">C: Course</span>
            <p name="started-date">1-2-2010</p>
            <p name="accessed-date">22-2-2011</p>
            <p name="complete-by-date">22-3-2011</p>
            <p name="diagnostic-score">0</p>
          </div>
          <div class="course-card" id="course-a"> 
            <span name="name">A: Course</span>
            <p name="started-date">22-1-2001</p>
            <p name="accessed-date">23-2-2011</p>
            <p name="passed-sections">0/6</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it('should correctly sort ascending by name', () => {
    // When
    sortCards('Course Name', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-c');
  });

  it('should correctly sort descending by name', () => {
    // When
    sortCards('Course Name', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-c');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort ascending by diagnostic score', () => {
    // When
    sortCards('Diagnostic Score', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-b');
  });

  it('should correctly sort descending by diagnostic score', () => {
    // When
    sortCards('Diagnostic Score', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-b');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort ascending by passed sections', () => {
    // When
    sortCards('Passed Sections', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-c');
    expect(newCards![1].id).toBe('course-a');
    expect(newCards![2].id).toBe('course-b');
  });

  it('should correctly sort descending by passed sections', () => {
    // When
    sortCards('Passed Sections', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-b');
    expect(newCards![1].id).toBe('course-a');
    expect(newCards![2].id).toBe('course-c');
  });

  it('should correctly sort ascending by enrolled date', () => {
    // When
    sortCards('Enrolled Date', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-c');
  });

  it('should correctly sort descending by enrolled date', () => {
    // When
    sortCards('Enrolled Date', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-c');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort ascending by last accessed date', () => {
    // When
    sortCards('Last Accessed Date', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-b');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort descending by last accessed date', () => {
    // When
    sortCards('Last Accessed Date', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-b');
  });

  it('should correctly sort ascending by complete by date', () => {
    // When
    sortCards('Complete By Date', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-c');
  });

  it('should correctly sort descending by complete by date', () => {
    // When
    sortCards('Complete By Date', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-c');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-a');
  });
});

describe('sortCards completed', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div id="course-cards">
          <div class="course-card" id="course-a"> 
            <span name="name">A: Course</span>
            <p name="started-date">31-1-2010</p>
            <p name="accessed-date">22-2-2010</p>
            <p name="completed-date">22-3-2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="course-card" id="course-b"> 
            <span name="name">B: Course</span>
            <p name="started-date">1-2-2010</p>
            <p name="accessed-date">22-2-2011</p>
            <p name="completed-date">22-3-2011</p>
            <p name="diagnostic-score">0</p>
          </div>
          <div class="course-card" id="course-c"> 
            <span name="name">C: Course</span>
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

  it('should correctly sort ascending by completed date', () => {
    // When
    sortCards('Completed Date', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-b');
  });

  it('should correctly sort descending by completed date', () => {
    // When
    sortCards('Completed Date', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-b');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-a');
  });
});

describe('sortCards available', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div id="course-cards">
          <div class="course-card" id="course-a"> 
            <span name="name">A: Course</span>
            <p name="brand">C: Brand</p>
            <p name="topic">Topic 2</p>
          </div>
          <div class="course-card" id="course-b"> 
            <span name="name">B: Course</span>
            <p name="brand">A: Brand</p>
            <p name="category">C: Category</p>
            <p name="topic">Topic 1</p>
          </div>
          <div class="course-card" id="course-c"> 
            <span name="name">C: Course</span>
            <p name="brand">B: Brand</p>
            <p name="category">B: Category</p>
          </div>
        </div>
      </body>
      </html>
    `).window.document;
  });

  it('should correctly sort ascending by brand', () => {
    // When
    sortCards('Brand', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-b');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort descending by brand', () => {
    // When
    sortCards('Brand', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-b');
  });

  it('should correctly sort ascending by category', () => {
    // When
    sortCards('Category', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-b');
  });

  it('should correctly sort descending by category', () => {
    // When
    sortCards('Category', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-b');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort ascending by topic', () => {
    // When
    sortCards('Topic', 'Ascending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-c');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort descending by topic', () => {
    // When
    sortCards('Topic', 'Descending');

    // Then
    const newCards = document.getElementById('course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-c');
  });
});
