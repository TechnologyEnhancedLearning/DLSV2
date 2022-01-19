import * as Checkboxes from '../checkboxes';

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

Checkboxes.default.setUpSelectAndDeselectInGroupButtons();
