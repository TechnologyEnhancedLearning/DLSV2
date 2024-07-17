window.addEventListener('load', () => {
  const button = document.getElementById('btn') as HTMLButtonElement | null;
  if (button) {
    button.style.display = 'block';
    button.addEventListener('click', () => {
      button.style.display = 'none';
    });
  }
});
