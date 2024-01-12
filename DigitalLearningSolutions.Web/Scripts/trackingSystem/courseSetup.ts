const copyLinkEls = document.getElementsByName('copy-course-link');

copyLinkEls.forEach((button) => {
  button.addEventListener('click', () => {
    const customisationId = button.id.substring(12);
    copyToClipboard(customisationId);
    removeExistingLinkCopiedText();
    const copyLinkButton = document.getElementById(button.id);
    if (copyLinkButton) {
      copyLinkButton.textContent = 'Copy course link - Link copied!';
    }
  });
});
function copyToClipboard(customisationId: string) {
  const rootPath = (<HTMLInputElement>document.getElementById('appRootPath')).value;
  const courseUrl = `${rootPath}/LearningMenu/${customisationId}`;
  const el = document.createElement('textarea');
  el.value = courseUrl;
  el.setAttribute('readonly', '');
  el.style.position = 'absolute';
  el.style.left = '-9999px';
  document.body.appendChild(el);
  el.select();
  document.execCommand('copy');
  document.body.removeChild(el);
}

function removeExistingLinkCopiedText() {
  Array.prototype.forEach.call(copyLinkEls, (el) => {
    if (el.textContent === 'Copy course link - Link copied!') {
      const copyLinkButton = document.getElementById(el.id);
      if (copyLinkButton) {
        copyLinkButton.textContent = 'Copy course link';
      }
    }
  });
}
