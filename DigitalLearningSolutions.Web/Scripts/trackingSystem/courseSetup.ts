const copyLinkEls = document.getElementsByName('copy-course-link');

copyLinkEls.forEach((button) => {
  button.addEventListener('click', () => {
    var customisationId = button.id.substring(12);
    copyToClipboard(customisationId);
    removeExistingLinkCopiedText();
    if (button) {
      button.textContent = 'Copy course link - Link copied!';
    }

  });
});
function copyToClipboard(customisationId: string) {
  var rootPath = (<HTMLInputElement>document.getElementById("appRootPath")).value;
  var courseUrl = rootPath + "/LearningMenu/" + customisationId;
  const el = document.createElement("textarea");
  el.value = courseUrl;
  el.setAttribute("readonly", "");
  el.style.position = "absolute";
  el.style.left = "-9999px";
  document.body.appendChild(el);
  el.select();
  document.execCommand("copy");
  document.body.removeChild(el);
}

function removeExistingLinkCopiedText() {
  Array.prototype.forEach.call(copyLinkEls, function (el) {
    if (el.textContent === "Copy course link - Link copied!") {
      if (el) {
        el.textContent = "Copy course link";
      }
    }
  });
}

