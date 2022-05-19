import { setupFullscreen } from './fullscreen';

declare global {
  interface Window {
    closeMpe: () => void;
  }
}

function postLearningCloseMpe(): void {
  // Extract the current domain, customisationId and sectionId out of the URL
  const matches = window.location.href.match(/^(.*)\/LearningMenu\/(\d+)\/(\d+)\/PostLearning\/Content#?$/);

  if (!matches || matches.length < 4) {
    return;
  }

  window.location.href = `${matches[1]}/LearningMenu/${matches[2]}/${matches[3]}/PostLearning`;
}

window.closeMpe = postLearningCloseMpe;
setupFullscreen();
