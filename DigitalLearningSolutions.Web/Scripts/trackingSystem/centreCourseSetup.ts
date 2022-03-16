import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const javascriptSearchEnabledElement = <HTMLInputElement>document.getElementById('javascript-search-enabled');
if (javascriptSearchEnabledElement?.value.trim() === 'true') {
  // eslint-disable-next-line no-new
  new SearchSortFilterAndPaginate('TrackingSystem/CourseSetup/AllCourseStatistics', true, true, true, 'CourseFilter');
}

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
  copyTextToClipboard(link);
}

function copyTextToClipboard(textToCopy: string): void {
  if (!navigator.clipboard) {
    const succeeded = copyTextToClipboardFallback(textToCopy);
    if (succeeded) {
      displaySuccessAlert(textToCopy);
    } else {
      displayFailureAlert(textToCopy);
    }
    return;
  }

  navigator.clipboard.writeText(textToCopy)
    .then(() => displaySuccessAlert(textToCopy))
    .catch(() => displayFailureAlert(textToCopy));
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

function displaySuccessAlert(text: string): void {
  // eslint-disable-next-line no-alert
  alert(`Copied the text: ${text}`);
}

function displayFailureAlert(text: string): void {
  // eslint-disable-next-line no-alert
  alert(`Copy not supported or blocked. Try manually selecting and copying: ${text}`);
}
