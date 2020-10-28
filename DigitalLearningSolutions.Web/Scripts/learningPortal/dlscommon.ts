function showLoader() {
  const spinEl = document.getElementById('loading-spinner');
  const mainEl = document.getElementById('maincontent');
  if (spinEl !== null && mainEl !== null) {
    spinEl.classList.remove('loading-spinner');
    mainEl.classList.add('loading-spinner');
  }
}

const els = Array.from(document.getElementsByClassName('trigger-loader'));
els.forEach((el) => {
  el.addEventListener('click', () => showLoader());
});
