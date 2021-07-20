const copyCourseLinkClass = 'copy-course-button';
const copyLinkIdPrefix = 'copy-course-';
const launchCourseButtonIdPrefix = 'launch-course-';

setupCourseLinkClipboardCopiers();

function setupCourseLinkClipboardCopiers() {
  const copyCourseLinks = Array.from(document.getElementsByClassName(copyCourseLinkClass));

  copyCourseLinks.forEach(
    (currentLink) => {
      const linkId = currentLink.id;
      const customisationId = linkId.slice(copyLinkIdPrefix.length);
      currentLink.addEventListener('click', () => { copyLaunchCourseLinkToClipboard(customisationId); });
    }
  );
}

function copyLaunchCourseLinkToClipboard(customisationId: string) {
  const launchCourseButtonId = launchCourseButtonIdPrefix + customisationId;
  const launchCourseButton = document.getElementById(launchCourseButtonId) as HTMLAnchorElement;
  const link = launchCourseButton.href;
  const succeeded = copyTextToClipboard(link);
  const alertMessage = succeeded
    ? `Copied the text: ${link}`
    : `Copy not supported or blocked. Try manually selecting and copying: ${link}`;

  alert(alertMessage);
}

function copyTextToClipboard(textToCopy: string): boolean {
  let succeeded: boolean;

  try {
    navigator.clipboard.writeText(textToCopy);
    succeeded = true;
  } catch (e) {
    succeeded = copyTextToClipboardFallback(textToCopy);
  }

  return succeeded;
}

function copyTextToClipboardFallback(textToCopy: string): boolean {
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
