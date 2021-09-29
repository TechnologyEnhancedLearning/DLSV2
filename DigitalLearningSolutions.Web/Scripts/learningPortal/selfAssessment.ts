function onSliderUpdate(inputElement: HTMLInputElement) {
  const selectedRatio = parseInt(inputElement.value, 10) / parseInt(inputElement.max, 10);
  // eslint-disable-next-line no-param-reassign
  inputElement.style.background = `linear-gradient(to right, #005eb8 0%, #005eb8 ${100 * selectedRatio}%, #fff ${100 * selectedRatio}%, white 100%)`;
  const output = document.querySelector(`span[for="${inputElement.id}"]`);
  if (output !== null) {
    output.textContent = inputElement.value;
  }
}

const inputs = Array.from(document.getElementsByTagName('input'));
inputs.forEach((e) => onSliderUpdate(e));
inputs.forEach((e) => {
  e.addEventListener('change', () => onSliderUpdate(e));
});

setUpSelectAndDeselectButtons();

function setUpSelectAndDeselectButtons(): void {
  const selectAllButton = document.getElementById('select-all-button') as HTMLAnchorElement;
  const deselectAllButton = document.getElementById('deselect-all-button') as HTMLAnchorElement;

  selectAllButton.addEventListener('click',
    () => {
      const group = selectAllButton.getAttribute('data-group') as string;
      selectAll(group);
    });

  deselectAllButton.addEventListener('click',
    () => {
      const group = deselectAllButton.getAttribute('data-group') as string;
      deselectAll(group);
    });
}

function selectAll(group: string): void {
  const allCheckboxes = document.querySelectorAll('.select-all-checkbox') as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => {
    if (checkbox.getAttribute('data-group') === group) {
      if (!checkbox.checked) checkbox.click();
    }
  });
}

function deselectAll(group: string): void {
  const allCheckboxes = document.querySelectorAll('.select-all-checkbox') as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => {
    if (checkbox.getAttribute('data-group') === group) {
      if (checkbox.checked) checkbox.click();
    }
  });
}
