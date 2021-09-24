import { selectAll, deselectAll } from '../common';

function setUpSelectAllButtons() {
  const submits = Array.from(
    document.querySelectorAll('.select-all-button'),
  );
  submits.forEach((submit) => {
    const element = <HTMLInputElement>submit;
    element.addEventListener('click', (e) => {
      e.preventDefault();
      const checkboxSelectorClass = `.${element.value.replace('select-all', 'checkbox')}`;
      selectAll(checkboxSelectorClass);
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
      deselectAll(checkboxSelectorClass);
    });
  });
}

setUpSelectAllButtons();
setUpDeselectAllButtons();
