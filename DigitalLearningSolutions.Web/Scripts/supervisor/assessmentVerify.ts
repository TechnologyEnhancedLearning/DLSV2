const selectRadioButtons = document.getElementsByName('SignedOff');
selectRadioButtons.forEach((button) => {
  button.addEventListener('click', () => {
    const radioValue = button.getAttribute('value') as string;
    const txtArea = document.getElementById('SupervisorComments');
    if (radioValue === 'false') {
      if (txtArea) {
        txtArea.setAttribute('required', 'required');
      }
    } else if (txtArea) {
      txtArea.removeAttribute('required');
    }
  });
});
