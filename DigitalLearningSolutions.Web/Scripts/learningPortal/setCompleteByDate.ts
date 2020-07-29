export function onDateChange() {
  const newDay = getDay();
  const newMonth = getMonth();
  const newYear = getYear();

  clearAllFieldsError();

  if (newDay !== undefined && (newDay < 1 || newDay > 31 || Number.isNaN(newDay))) {
    displayValidationError('Please enter a valid day');
    return;
  }

  if (newMonth !== undefined && (newMonth < 1 || newMonth > 12 || Number.isNaN(newMonth))) {
    displayValidationError('Please enter a valid month');
    return;
  }

  if (newYear !== undefined && (newYear < 2000 || newYear > 3000 || Number.isNaN(newYear))) {
    displayValidationError('Please enter a valid year');
    return;
  }

  if (newDay && newMonth && newYear && dateInvalid(newDay, newMonth, newYear)) {
    displayGeneralValidationError('Please enter a valid date');
    return;
  }

  clearValidationError();
}

function displayGeneralValidationError(message: string) {
  displayValidationError(message);
  markAllFieldsAsError();
}

function displayValidationError(message: string) {
  const validationMessageElement = document.getElementById('validation-message');
  if (validationMessageElement !== null) {
    validationMessageElement.innerText = message;
    validationMessageElement.classList.add('nhsuk-error-message');
    validationMessageElement.removeAttribute('hidden');
  }

  const formGroup = document.getElementById('form-group');
  if (formGroup !== null) {
    formGroup.classList.add('nhsuk-form-group--error');
  }

  const saveButton = document.getElementById('save-button');
  if (saveButton !== null) {
    saveButton.setAttribute('disabled', '');
  }
}

function markAllFieldsAsError() {
  const dayField = document.getElementById('day');
  const monthField = document.getElementById('month');
  const yearField = document.getElementById('year');

  if (dayField && monthField && yearField) {
    dayField.classList.add('nhsuk-input--error');
    monthField.classList.add('nhsuk-input--error');
    yearField.classList.add('nhsuk-input--error');
  }
}

function clearAllFieldsError() {
  const dayField = document.getElementById('day');
  const monthField = document.getElementById('month');
  const yearField = document.getElementById('year');

  if (dayField && monthField && yearField) {
    dayField.classList.remove('nhsuk-input--error');
    monthField.classList.remove('nhsuk-input--error');
    yearField.classList.remove('nhsuk-input--error');
  }
}

function clearValidationError() {
  const validationMessageElement = document.getElementById('validation-message');
  if (validationMessageElement !== null) {
    validationMessageElement.classList.remove('nhsuk-error-message');
    validationMessageElement.setAttribute('hidden', '');
  }

  const formGroup = document.getElementById('form-group');
  if (formGroup !== null) {
    formGroup.classList.remove('nhsuk-form-group--error');
  }

  const saveButton = document.getElementById('save-button');
  if (saveButton !== null) {
    saveButton.removeAttribute('disabled');
  }
}

export function dateInvalid(day: number, month: number, year: number) {
  const date = new Date(year, month - 1, day);
  return date.getDate() !== day;
}

function getDay() {
  return getInputValue('day');
}

function getMonth() {
  return getInputValue('month');
}

function getYear() {
  return getInputValue('year');
}

function getInputValue(inputId: string): number | undefined {
  const element = <HTMLInputElement>document.getElementById(inputId);
  if (element.value === '') {
    return undefined;
  }
  return parseInt(element?.value, 10);
}

function registerListeners() {
  document.getElementById('day')?.addEventListener('change', onDateChange);
  document.getElementById('month')?.addEventListener('change', onDateChange);
  document.getElementById('year')?.addEventListener('change', onDateChange);
}

registerListeners();
