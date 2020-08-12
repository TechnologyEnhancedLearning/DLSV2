import { JSDOM } from 'jsdom';

// @ts-ignore
global.document = {
  getElementById: () => null,
  getElementsByClassName: () => [] as any,
};

// eslint-disable-next-line import/first
import { sortCards, getSortValue } from '../learningPortal/sortCourses';

describe('getSortValue', () => {
  it('should correctly extract the course name field', () => {
    // Given
    const expectedName = 'example name';
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="current-course-card"> 
          <span name="name">${expectedName}</span>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualName = getSortValue(courseCard, 'Course Name');

    // Then
    expect(actualName).toEqual(expectedName);
  });

  it('should correctly extract the started date field', () => {
    // Given
    const startedDateString = '01/01/2020';
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="current-course-card"> 
          <p name="started-date">${startedDateString}</p>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualStartedDate = getSortValue(courseCard, 'Enrolled Date');

    // Then
    expect(actualStartedDate).toEqual(new Date(startedDateString));
  });

  it('should correctly extract the accessed date field', () => {
    // Given
    const accessedDateString = '02/02/2020';
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="current-course-card"> 
          <p name="accessed-date">${accessedDateString}</p>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualAccessedDate = getSortValue(courseCard, 'Last Accessed Date');

    // Then
    expect(actualAccessedDate).toEqual(new Date(accessedDateString));
  });

  it('should correctly extract the complete by date field', () => {
    // Given
    const completeByDateString = '03/03/2020';
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="current-course-card"> 
          <p name="complete-by-date">${completeByDateString}</p>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualCompleteByDate = getSortValue(courseCard, 'Complete By Date');

    // Then
    expect(actualCompleteByDate).toEqual(new Date(completeByDateString));
  });

  it('should correctly extract the diagnostic score field', () => {
    // Given
    const expectedDiagnosticScore = 6;
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="current-course-card"> 
          <p name="diagnostic-score">${expectedDiagnosticScore}/10</p>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualDiagnosticScore = getSortValue(courseCard, 'Diagnostic Score');

    // Then
    expect(actualDiagnosticScore).toEqual(expectedDiagnosticScore);
  });

  it('should correctly extract the passed sections field', () => {
    // Given
    const expectedPassedSections = 8;
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="current-course-card"> 
          <p name="passed-sections">${expectedPassedSections}/10</p>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualPassedSections = getSortValue(courseCard, 'Passed Sections');

    // Then
    expect(actualPassedSections).toEqual(expectedPassedSections);
  });

  it('should correctly extract the complete by date field with null data', () => {
    // Given
    const expectedCompleteByDate = new Date(0);
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="current-course-card">
          <p name="complete-by-date">-</p>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualCompleteByDate = getSortValue(courseCard, 'Complete By Date');

    // Then
    expect(actualCompleteByDate).toEqual(expectedCompleteByDate);
  });

  it('should correctly extract the diagnostic score field with null data', () => {
    // Given
    const expectedDiagnosticScore = -1;
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="current-course-card">
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualDiagnosticScore = getSortValue(courseCard, 'Diagnostic Score');

    // Then
    expect(actualDiagnosticScore).toEqual(expectedDiagnosticScore);
  });

  it('should correctly extract the passed sections field with null data', () => {
    // Given
    const expectedPassedSections = -1;
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="current-course-card">
        </div>
      </body>
      </html>
    `).window.document;

    // When
    const courseCard = document.getElementsByClassName('current-course-card')[0];
    const actualPassedSections = getSortValue(courseCard, 'Passed Sections');

    // Then
    expect(actualPassedSections).toEqual(expectedPassedSections);
  });
});

describe('sortCards', () => {
  beforeEach(() => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div id="current-course-cards">
          <div class="current-course-card" id="course-b"> 
            <span name="name">B: Course</span>
            <p name="started-date">31-1-2010</p>
            <p name="accessed-date">22-2-2010</p>
            <p name="complete-by-date">22-3-2010</p>
            <p name="diagnostic-score">123</p>
            <p name="passed-sections">4/6</p>
          </div>
          <div class="current-course-card" id="course-c"> 
            <span name="name">C: Course</span>
            <p name="started-date">1-2-2010</p>
            <p name="accessed-date">22-2-2011</p>
            <p name="complete-by-date">22-3-2011</p>
            <p name="diagnostic-score">0</p>
          </div>
          <div class="current-course-card" id="course-a"> 
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
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-c');
  });

  it('should correctly sort descending by name', () => {
    // When
    sortCards('Course Name', 'Descending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-c');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort ascending by diagnostic score', () => {
    // When
    sortCards('Diagnostic Score', 'Ascending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-b');
  });

  it('should correctly sort descending by diagnostic score', () => {
    // When
    sortCards('Diagnostic Score', 'Descending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-b');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort ascending by passed sections', () => {
    // When
    sortCards('Passed Sections', 'Ascending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-c');
    expect(newCards![1].id).toBe('course-a');
    expect(newCards![2].id).toBe('course-b');
  });

  it('should correctly sort descending by passed sections', () => {
    // When
    sortCards('Passed Sections', 'Descending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-b');
    expect(newCards![1].id).toBe('course-a');
    expect(newCards![2].id).toBe('course-c');
  });

  it('should correctly sort ascending by enrolled date', () => {
    // When
    sortCards('Enrolled Date', 'Ascending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-c');
  });

  it('should correctly sort descending by enrolled date', () => {
    // When
    sortCards('Enrolled Date', 'Descending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-c');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort ascending by last accessed date', () => {
    // When
    sortCards('Last Accessed Date', 'Ascending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-b');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-a');
  });

  it('should correctly sort descending by last accessed date', () => {
    // When
    sortCards('Last Accessed Date', 'Descending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-c');
    expect(newCards![2].id).toBe('course-b');
  });

  it('should correctly sort ascending by complete by date', () => {
    // When
    sortCards('Complete By Date', 'Ascending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-a');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-c');
  });

  it('should correctly sort descending by complete by date', () => {
    // When
    sortCards('Complete By Date', 'Descending');

    // Then
    const newCards = document.getElementById('current-course-cards')?.children;
    expect(newCards?.length).toEqual(3);
    expect(newCards![0].id).toBe('course-c');
    expect(newCards![1].id).toBe('course-b');
    expect(newCards![2].id).toBe('course-a');
  });
});
