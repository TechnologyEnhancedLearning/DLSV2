export function setupFullscreen(): void {
  getExitFullscreenButton()?.addEventListener(
    'click',
    () => exitFullscreen()
  );
  getEnterFullscreenButton()?.addEventListener(
    'click',
    () => enterFullscreen()
  );
}

export function exitFullscreen(): void {
  getIFrame()?.classList.remove('fullscreen');
  getExitFullscreenButton()?.classList.add('hidden');
  getEnterFullscreenButton()?.classList.remove('hidden');
  getHeader()?.classList.remove('hidden');
  getFooter()?.classList.remove('hidden');
  getBreadcrumbs()?.classList.remove('hidden');
}

export function enterFullscreen(): void {
  getIFrame()?.classList.add('fullscreen');
  getExitFullscreenButton()?.classList.remove('hidden');
  getEnterFullscreenButton()?.classList.add('hidden');
  getHeader()?.classList.add('hidden');
  getFooter()?.classList.add('hidden');
  getBreadcrumbs()?.classList.add('hidden');
}

function getEnterFullscreenButton(): HTMLElement | undefined {
  return <HTMLElement>document.getElementById('content-viewer_enter-fullscreen-button');
}

function getExitFullscreenButton(): HTMLElement | undefined {
  return <HTMLElement>document.getElementById('content-viewer_exit-fullscreen-button');
}

function getIFrame(): HTMLElement | undefined {
  return <HTMLElement>document.getElementById('content-viewer_iframe');
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
