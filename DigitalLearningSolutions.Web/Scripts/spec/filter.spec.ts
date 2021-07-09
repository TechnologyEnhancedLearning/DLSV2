/* eslint-disable @typescript-eslint/no-non-null-assertion,@typescript-eslint/ban-ts-comment */
import { JSDOM } from 'jsdom';
import {AppliedFilterTag, filterSearchableElements, filterSeparator} from '../searchSortAndPaginate/filter';
import getSearchableElements from './getSearchableElements';

describe('filter', () => {
  it('should show all results with no filter', () => {
    // Given
    createFilterableElements('');

    // When
    const filteredElements = filterSearchableElements(getSearchableElements(), getPossibleFilters());

    // Then
    expect(filteredElements.length).toBe(3);
    expect(filteredElements[0].title).toBe('a: Course');
  });

  it('should return expected results with single filter', () => {
    // Given
    createFilterableElements('Number|Number|2');

    // When
    const filteredElements = filterSearchableElements(getSearchableElements(), getPossibleFilters());

    // Then
    expect(filteredElements.length).toBe(2);
    expect(filteredElements[0].title).toBe('a: Course');
  });

  it('should return expected results with 2 filters in different groups', () => {
    // Given
    createFilterableElements(`Number|Number|2${filterSeparator}Name|Name|b`);
    const test = document.documentElement.innerHTML;

    // When
    const filteredElements = filterSearchableElements(getSearchableElements(), getPossibleFilters());

    // Then
    expect(filteredElements.length).toBe(1);
    expect(filteredElements[0].title).toBe('b: Course');
  });
});

function getPossibleFilters(): AppliedFilterTag[] {
  const tags = Array.from(document.getElementsByClassName('filter-tag'));
  return tags.map((element) => ({
    element,
    filterValue: element.getAttribute('data-filter-value')?.trim() ?? '',
  }));
}

function createFilterableElements(filterBy: string) {
  global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <input type="text" id="filter-by" value="${filterBy}"/>
        <div id="applied-filters">
            <div class="applied-filter-container" id="applied-filter-container"></div>
        </div>
        <span id="page-indicator"></span>
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
            <span name="name" class="searchable-element-title">a: Course</span>
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
        <div class="filter-tag" data-filter-value="Number|Number|1">Number: 1</div>
        <div class="filter-tag" data-filter-value="Number|Number|2">Number: 2</div>
        <div class="nhsuk-pagination-item--previous"></div>
        <div class="nhsuk-pagination-item--next"></div>
      </body>
      </html>
    `).window.document;
}
