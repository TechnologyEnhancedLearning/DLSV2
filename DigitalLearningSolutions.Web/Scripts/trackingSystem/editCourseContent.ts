function selectAll(selectorClass: string): void {
  const allCheckboxes = document.querySelectorAll(selectorClass)as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => {
    if (!checkbox.checked) checkbox.click();
  });
}

function deselectAll(selectorClass: string): void {
  const allCheckboxes = document.querySelectorAll(selectorClass) as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => {
    if (checkbox.checked) checkbox.click();
  });
}

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
