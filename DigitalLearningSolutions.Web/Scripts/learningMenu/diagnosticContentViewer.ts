import { setupFullscreen } from './fullscreen';

function diagnosticCloseMpe(): void {
  // Extract the current domain, customisationId and sectionId out of the URL
  const matches = window.location.href.match(/^(.*)\/LearningMenu\/(\d+)\/(\d+)\/Diagnostic\/Content(\?checkedTutorials=\d+(&checkedTutorials=\d+)*)?#?$/);

  if (!matches || matches.length < 4) {
    return;
  }

  window.location.href = `${matches[1]}/LearningMenu/${matches[2]}/${matches[3]}/Diagnostic`;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
(window as any).closeMpe = diagnosticCloseMpe;
setupFullscreen();
