import * as Checkboxes from '../checkboxes';

function setUpSelectAllButtons() {
  const submits = Array.from(
    document.querySelectorAll('.select-all-button'),
  );
  submits.forEach((submit) => {
    const element = <HTMLInputElement>submit;
    element.addEventListener('click', (e) => {
      e.preventDefault();
      const checkboxSelectorClass = `.${element.value.replace('select-all', 'checkbox')}`;
      Checkboxes.default.selectAll(checkboxSelectorClass);
    });
  });
}

function setUpDeselectAllButtons() {
  const submits = Array.from(
    document.querySelectorAll('.deselect-all-button'),
  );
  submits.forEach((submit) => {
    const element = <HTMLInputElement>submit;
    element.addEventListener('click', (e) => {
      e.preventDefault();
      const checkboxSelectorClass = `.${element.value.replace('deselect-all', 'checkbox')}`;
      Checkboxes.default.deselectAll(checkboxSelectorClass);
    });
  });
}

setUpSelectAllButtons();
setUpDeselectAllButtons();
