export function setupFullscreen(): void {
  getExitFullscreenButton()?.addEventListener(
    'click',
    () => exitFullscreen(),
  );
  getEnterFullscreenButton()?.addEventListener(
    'click',
    () => enterFullscreen(),
  );
}

export function exitFullscreen(): void {
  getIFrameWrapper()?.classList.remove('fullscreen');
  getIFrameWrapper()?.classList.add('content-viewer_iframe-wrapper');
  getIFrameWrapper()?.classList.add('nhsuk-u-margin-top-6');
  getExitFullscreenButton()?.classList.add('hidden');
  getEnterFullscreenButton()?.classList.remove('hidden');
  getHeader()?.classList.remove('hidden');
  getFooter()?.classList.remove('hidden');
  getBreadcrumbs()?.classList.remove('hidden');
  getHeading()?.classList.remove('hidden');
}

export function enterFullscreen(): void {
  getIFrameWrapper()?.classList.remove('content-viewer_iframe-wrapper');
  getIFrameWrapper()?.classList.remove('nhsuk-u-margin-top-6');
  getIFrameWrapper()?.classList.add('fullscreen');
  getExitFullscreenButton()?.classList.remove('hidden');
  getEnterFullscreenButton()?.classList.add('hidden');
  getHeader()?.classList.add('hidden');
  getFooter()?.classList.add('hidden');
  getBreadcrumbs()?.classList.add('hidden');
  getHeading()?.classList.add('hidden');
}

function getEnterFullscreenButton(): HTMLElement | undefined {
  return <HTMLElement>document.getElementById('content-viewer_enter-fullscreen-button');
}

function getExitFullscreenButton(): HTMLElement | undefined {
  return <HTMLElement>document.getElementById('content-viewer_exit-fullscreen-button');
}

function getIFrameWrapper(): HTMLElement | undefined {
  return <HTMLElement>document.getElementById('content-viewer_iframe-wrapper');
}

function getHeader(): HTMLElement | undefined {
  return <HTMLElement>document.getElementsByTagName('header')[0];
}

function getFooter(): HTMLElement | undefined {
  return <HTMLElement>document.getElementsByTagName('footer')[0];
}

function getBreadcrumbs(): HTMLElement | undefined {
  return <HTMLElement>document.getElementsByClassName('nhsuk-breadcrumb')[0];
}
function getHeading(): HTMLElement | undefined {
  return <HTMLElement>document.getElementById('page-heading');
}
