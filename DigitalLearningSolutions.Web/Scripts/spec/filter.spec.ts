/* eslint-disable @typescript-eslint/no-non-null-assertion,@typescript-eslint/ban-ts-comment */
import { JSDOM } from 'jsdom';
import { IAppliedFilterTag, filterSearchableElements, filterSeparator } from '../searchSortFilterAndPaginate/filter';
import getSearchableElements from './getSearchableElements';

describe('filter', () => {
  it('should show all results with no filter', () => {
    // Given
    createFilterableElements('');

    // When
    const filteredElements = filterSearchableElements(
      getSearchableElements(),
      getPossibleFilters(),
    );

    // Then
    expect(filteredElements.length).toBe(3);
    expect(filteredElements[0].searchableContent).toBe('a: Course');
  });

  it('should return expected results with single filter', () => {
    // Given
    createFilterableElements('Number|Number|2');

    // When
    const filteredElements = filterSearchableElements(
      getSearchableElements(),
      getPossibleFilters(),
    );

    // Then
    expect(filteredElements.length).toBe(2);
    expect(filteredElements[0].searchableContent).toBe('a: Course');
  });

  it('should return expected results with 2 filters in different groups', () => {
    // Given
    createFilterableElements(`Number|Number|2${filterSeparator}Name|Name|b`);

    // When
    const filteredElements = filterSearchableElements(
      getSearchableElements(),
      getPossibleFilters(),
    );

    // Then
    expect(filteredElements.length).toBe(1);
    expect(filteredElements[0].searchableContent).toBe('b: Course');
  });

  it('should return expected results with 2 filters in the same group', () => {
    // Given
    createFilterableElements(`Name|Name|a${filterSeparator}Name|Name|c`);

    // When
    const filteredElements = filterSearchableElements(
      getSearchableElements(),
      getPossibleFilters(),
    );

    // Then
    expect(filteredElements.length).toBe(2);
    expect(filteredElements[0].searchableContent).toBe('a: Course');
  });

  it('should return expected results with a mix of grouped and ungrouped filters', () => {
    // Given
    createFilterableElements(
      `Name|Name|a${filterSeparator}Name|Name|c${filterSeparator}Number|Number|1`,
    );

    // When
    const filteredElements = filterSearchableElements(
      getSearchableElements(),
      getPossibleFilters(),
    );

    // Then
    expect(filteredElements.length).toBe(1);
    expect(filteredElements[0].searchableContent).toBe('c: Course');
  });
});

describe('applied filters', () => {
  it('should be hidden with no filter', () => {
    // Given
    createFilterableElements('');

    // When
    filterSearchableElements(getSearchableElements(), getPossibleFilters());

    // Then
    const appliedFilters = document.getElementById('applied-filters');
    expect(appliedFilters?.hidden).toBeTruthy();
  });

  it('should be visible with a filter', () => {
    // Given
    createFilterableElements('Number|Number|2');

    // When
    filterSearchableElements(getSearchableElements(), getPossibleFilters());

    // Then
    const appliedFilters = document.getElementById('applied-filters');
    expect(appliedFilters?.hidden).toBeFalsy();
  });
});

describe('applied filter container', () => {
  it('should have the same number of children as there are filters', () => {
    // Given
    createFilterableElements(
      `Name|Name|a${filterSeparator}Name|Name|c${filterSeparator}Number|Number|1`,
    );

    // When
    filterSearchableElements(getSearchableElements(), getPossibleFilters());

    // Then
    const appliedFilters = document.getElementById('applied-filter-container');
    expect(appliedFilters?.children.length).toBe(3);
  });
});

function getPossibleFilters(): IAppliedFilterTag[] {
  const tags = Array.from(document.getElementsByClassName('filter-tag'));
  return tags.map((element) => ({
    element,
    filterValue: element.getAttribute('data-filter-value')?.trim() ?? '',
  }));
}

function createFilterableElements(existingFilterString: string) {
  global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <input type="text" id="existing-filter-string" value="${existingFilterString}"/>
        <div id="applied-filters" hidden>
            <div class="applied-filter-container" id="applied-filter-container"></div>
        </div>
        <span class="page-indicator"></span>
        <div id="searchable-elements">
          <div class="searchable-element" id="course-a">
            <span name="name" class="searchable-element-title">a: Course</span>
              <div class="card-filter-tag" data-filter-value="Name|Name|a">
                <strong>Name: a</strong>
              </div>
              <div class="card-filter-tag" data-filter-value="Number|Number|2">
                <strong>Number: 2</strong>
              </div>
          </div>
          <div class="searchable-element" id="course-b">
            <span name="name" class="searchable-element-title">b: Course</span>
            <div class="card-filter-tag" data-filter-value="Name|Name|b">
              <strong>Name: b</strong>
            </div>
            <div class="card-filter-tag" data-filter-value="Number|Number|2">
              <strong>Number: 2</strong>
            </div>
          </div>
          <div class="searchable-element" id="course-c">
            <span name="name" class="searchable-element-title">c: Course</span>
            <div class="card-filter-tag" data-filter-value="Name|Name|c">
              <strong>Name: c</strong>
            </div>
            <div class="card-filter-tag" data-filter-value="Number|Number|1">
              <strong>Number: 1</strong>
            </div>
          </div>
        </div>
        <div class="filter-tag" data-filter-value="Name|Name|a">Name: a</div>
        <div class="filter-tag" data-filter-value="Name|Name|b">Name: b</div>
        <div class="filter-tag" data-filter-value="Name|Name|c">Name: c</div>
        <div class="filter-tag" data-filter-value="Number|Number|1">Number: 1</div>
        <div class="filter-tag" data-filter-value="Number|Number|2">Number: 2</div>
        <div class="nhsuk-pagination-item--previous"></div>
        <div class="nhsuk-pagination-item--next"></div>
      </body>
      </html>
    `).window.document;
}
