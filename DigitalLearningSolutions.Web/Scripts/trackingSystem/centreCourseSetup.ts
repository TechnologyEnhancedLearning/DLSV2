const copyCourseLinkClass = 'copy-course-button';
const copyLinkIdPrefix = 'copy-course-';
const launchCourseButtonIdPrefix = 'launch-course-';

setUpCourseLinkClipboardCopiers();

function setUpCourseLinkClipboardCopiers() {
  const copyCourseLinks = Array.from(document.getElementsByClassName(copyCourseLinkClass));

  copyCourseLinks.forEach(
    (currentLink) => {
      const linkId = currentLink.id;
      const customisationId = linkId.slice(copyLinkIdPrefix.length);
      currentLink.addEventListener('click', () => copyLaunchCourseLinkToClipboard(customisationId));
    },
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
  try {
    navigator.clipboard.writeText(textToCopy);
    return true;
  } catch (e) {
    return copyTextToClipboardFallback(textToCopy);
  }
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
