const copyCourseLinkClass = 'copy-course-link';
const copyLinkIdPrefix = 'copy-course-';
const launchCourseButtonIdPrefix = 'launch-course-';

setupCourseLinkClipboardCopiers();

function setupCourseLinkClipboardCopiers() {
  const copyCourseLinks = document.getElementsByClassName(copyCourseLinkClass);

  for (let i = 0; i < copyCourseLinks.length; i += 1) {
    const currentLink = copyCourseLinks[i];
    const linkId = currentLink.id;
    const customisationId = linkId.slice(copyLinkIdPrefix.length);

    currentLink.addEventListener('click', (e) => {
      e.preventDefault();
      e.stopPropagation();
      copyLaunchCourseLinkToClipboard(customisationId);
    });
  }
}

function copyLaunchCourseLinkToClipboard(customisationId: string) {
  const launchCourseButtonId = launchCourseButtonIdPrefix + customisationId;
  const copyText = document.getElementById(launchCourseButtonId) as HTMLAnchorElement;
  const link = copyText.href;
  const succeeded = copyTextToClipboard(link);
  let alertMessage: string;

  if (succeeded) {
    alertMessage = `Copied the text: ${link}`;
  } else {
    alertMessage = `Copy not supported or blocked. Try manually selecting and copying: ${link}`;
  }

  alert(alertMessage);
}

function copyTextToClipboard(textToCopy: string) : boolean {
  const hiddenInput = document.body.appendChild(document.createElement('input'));
  hiddenInput.value = textToCopy;
  hiddenInput.select();
  hiddenInput.setSelectionRange(0, textToCopy.length);
  let succeeded: boolean;

  try {
    succeeded = document.execCommand('copy');
  } catch (e) {
    succeeded = false;
  }

  document.body.removeChild(hiddenInput);
  return succeeded;
}
