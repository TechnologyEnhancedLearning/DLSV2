import { setupFullscreen } from './fullscreen';
import { keepSessionAlive, heartbeatInterval } from './keepSessionAlive';

function closeMpe(): void {
  // Extract the current domain, customisationId, sectionId and tutorialId out of the URL
  const matches = window.location.href.match(/^(.*)\/LearningMenu\/(\d+)\/(\d+)\/(\d+)\/Tutorial#?$/);

  if (!matches || matches.length < 5) {
    return;
  }

  window.location.href = `${matches[1]}/LearningMenu/${matches[2]}/${matches[3]}/${matches[4]}`;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
(window as any).closeMpe = closeMpe;
setupFullscreen();

// send out a heartbeat, to keep this session alive, once a minute
setInterval(keepSessionAlive, heartbeatInterval);
