import { JSDOM } from 'jsdom';

// @ts-ignore
global.document = {
  getElementById: () => null,
};
// eslint-disable-next-line import/first
import { dateInvalid, onDateChange } from '../learningPortal/setCompleteByDate';

describe('dateInvalid', () => {
  it('Returns true for a non-existant date', () => {
    // Given

    // When
    const result = dateInvalid(31, 2, 2020);

    // Then
    expect(result).toBeTrue();
  });

  it('Returns false for a valid date', () => {
    // Given

    // When
    const result = dateInvalid(31, 3, 2020);

    // Then
    expect(result).toBeFalse();
  });
});

describe('onDateChange', () => {
  it('Rejects an invalid day', () => {
    // given
    global.document = new JSDOM(`<html>
      <head></head>
      <body>
      <form>
          <input name="day" type="number" value="99" id="day"/>
          <input name="month" type="number" value="1" id="month"/>
          <input name="year" type="number" value="2020" id="year"/>
      </form>
      <span id="validation-message"></span>
      </body>
      </html>
    `).window.document;

    // When
    onDateChange();
    const actualValidationMessage = global.document.getElementById('validation-message')?.innerText;
    const expectedValidationMessage = 'Please enter a valid day';

    // Then
    expect(actualValidationMessage).toBe(expectedValidationMessage);
  });

  it('Rejects an invalid month', () => {
    // given
    global.document = new JSDOM(`<html>
      <head></head>
      <body>
      <form>
          <input name="day" type="number" value="1" id="day"/>
          <input name="month" type="number" value="21" id="month"/>
          <input name="year" type="number" value="2020" id="year"/>
      </form>
      <span id="validation-message"></span>
      </body>
      </html>
    `).window.document;

    // When
    onDateChange();
    const actualValidationMessage = global.document.getElementById('validation-message')?.innerText;
    const expectedValidationMessage = 'Please enter a valid month';

    // Then
    expect(actualValidationMessage).toBe(expectedValidationMessage);
  });

  it('Rejects an invalid year', () => {
    // given
    global.document = new JSDOM(`<html>
      <head></head>
      <body>
      <form>
          <input name="day" type="number" value="1" id="day"/>
          <input name="month" type="number" value="1" id="month"/>
          <input name="year" type="number" value="220" id="year"/>
      </form>
      <span id="validation-message"></span>
      </body>
      </html>
    `).window.document;

    // When
    onDateChange();
    const actualValidationMessage = global.document.getElementById('validation-message')?.innerText;
    const expectedValidationMessage = 'Please enter a valid year';

    // Then
    expect(actualValidationMessage).toBe(expectedValidationMessage);
  });

  it('Displays no validation message when the date is valid', () => {
    // given
    global.document = new JSDOM(`<html>
      <head></head>
      <body>
      <form>
          <input name="day" type="number" value="1" id="day"/>
          <input name="month" type="number" value="1" id="month"/>
          <input name="year" type="number" value="2020" id="year"/>
      </form>
      <span id="validation-message"></span>
      </body>
      </html>
    `).window.document;

    // When
    onDateChange();
    const actualValidationMessage = global.document.getElementById('validation-message')?.innerText;

    // Then
    expect(actualValidationMessage).toBeUndefined();
  });

  it('Disables the submit button when the date is invalid', () => {
    // given
    global.document = new JSDOM(`<html>
      <head>
      </head>
      <body>
      <form>
          <input name="day" type="number" value="1" id="day"/>
          <input name="month" type="number" value="1" id="month"/>
          <input name="year" type="number" value="220" id="year"/>
          <button id="save-button" type="submit"/>
      </form>
      <span id="validation-message"></span>
      </body>
      </html>
    `).window.document;

    // When
    onDateChange();
    const buttonIsDisabled = global.document.getElementById('save-button')?.hasAttribute('disabled');

    // Then
    expect(buttonIsDisabled).toBeTrue();
  });

  it('Enables the submit button when the date is valid', () => {
    // given
    global.document = new JSDOM(`<html>
      <head>
      </head>
      <body>
      <form>
          <input name="day" type="number" value="1" id="day"/>
          <input name="month" type="number" value="1" id="month"/>
          <input name="year" type="number" value="2020" id="year"/>
          <button id="save-button" type="submit" disabled />
      </form>
      <span id="validation-message"></span>
      </body>
      </html>
    `).window.document;

    // When
    onDateChange();
    const buttonIsDisabled = global.document.getElementById('save-button')?.hasAttribute('disabled');

    // Then
    expect(buttonIsDisabled).toBeFalse();
  });
});
