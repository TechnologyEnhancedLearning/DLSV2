/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { JSDOM } from 'jsdom';
import { exitFullscreen, enterFullscreen } from '../../learningMenu/fullscreen';

describe('enterFullscreen', () => {
  it('should show the exit fullscreen button', () => {
    // Given
    global.document = new JSDOM(`
    <html>
    <head></head>
    <body>
      <a class="nhsuk-button nhsuk-button--secondary exit-fullscreen-button hidden" role="button" id="content-viewer_exit-fullscreen-button" href="#">
        Exit fullscreen
      </a>
    </body>
    </html>
  `).window.document;

    // When
    enterFullscreen();

    // Then
    const exitButton = document.getElementById('content-viewer_exit-fullscreen-button');
    expect(exitButton!.classList).not.toContain('hidden');
  });

  it('should hide the enter fullscreen button', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <a class="nhsuk-button js-only-inline" role="button" id="content-viewer_enter-fullscreen-button" href="#">
          Fullscreen
        </a>
      </body>
      </html>
    `).window.document;

    // When
    enterFullscreen();

    // Then
    const fullscreenButton = document.getElementById('content-viewer_enter-fullscreen-button');
    expect(fullscreenButton!.classList).toContain('hidden');
  });

  it('should hide the header', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <header class="nhsuk-header nhsuk-header--organisation nhsuk-header--white" role="banner">
        </header>
      </body>
      </html>
    `).window.document;

    // When
    enterFullscreen();

    // Then
    const headerElement = document.getElementsByTagName('header')[0];
    expect(headerElement!.classList).toContain('hidden');
  });

  it('should hide the footer', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <footer role="contentinfo" class>
        </footer>
      </body>
      </html>
    `).window.document;

    // When
    enterFullscreen();

    // Then
    const footer = document.getElementsByTagName('footer')[0];
    expect(footer!.classList).toContain('hidden');
  });

  it('should hide the breadcrumbs', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <nav class="nhsuk-breadcrumb" aria-label="Breadcrumb">
        </nav>
      </body>
      </html>
    `).window.document;

    // When
    enterFullscreen();

    // Then
    const breadcrumbs = document.getElementsByClassName('nhsuk-breadcrumb')[0];
    expect(breadcrumbs!.classList).toContain('hidden');
  });

  it('should make wrapper fullscreen', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="nhsuk-u-margin-bottom-6 content-viewer_iframe-wrapper" id="content-viewer_iframe-wrapper">
          <iframe
            src="https://www.dls.nhs.uk/CMS/CMSContent/Course324/Section1058/Tutorials/01-01-CreateaPresentation/itspplayer.html?CentreID=101&amp;CustomisationID=19068&amp;TutorialID=4671&amp;CandidateID=254480&amp;Version=2&amp;ProgressID=285049&amp;type=learn&amp;TrackURL=https://localhost:44367/tracking/tracker"
            class="nhsuk-u-margin-bottom-6 js-only-block content-viewer_iframe">
          </iframe>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    enterFullscreen();

    // Then
    const wrapper = document.getElementById('content-viewer_iframe-wrapper');
    expect(wrapper!.classList).toContain('fullscreen');
  });

  it('should remove the wrapper class', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="nhsuk-u-margin-bottom-6 content-viewer_iframe-wrapper" id="content-viewer_iframe-wrapper">
          <iframe
            src="https://www.dls.nhs.uk/CMS/CMSContent/Course324/Section1058/Tutorials/01-01-CreateaPresentation/itspplayer.html?CentreID=101&amp;CustomisationID=19068&amp;TutorialID=4671&amp;CandidateID=254480&amp;Version=2&amp;ProgressID=285049&amp;type=learn&amp;TrackURL=https://localhost:44367/tracking/tracker"
            class="nhsuk-u-margin-bottom-6 js-only-block content-viewer_iframe">
          </iframe>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    enterFullscreen();

    // Then
    const wrapper = document.getElementById('content-viewer_iframe-wrapper');
    expect(wrapper!.classList).not.toContain('content-viewer_iframe-wrapper');
  });
});

describe('exitFullscreen', () => {
  it('should hide the exit fullscreen button', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <a class="nhsuk-button nhsuk-button--secondary exit-fullscreen-button" role="button" id="content-viewer_exit-fullscreen-button" href="#">
          Exit fullscreen
        </a>
      </body>
      </html>
    `).window.document;

    // When
    exitFullscreen();

    // Then
    const exitButton = document.getElementById('content-viewer_exit-fullscreen-button');
    expect(exitButton!.classList).toContain('hidden');
  });

  it('should show the enter fullscreen button', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <a class="nhsuk-button js-only-inline hidden" role="button" id="content-viewer_enter-fullscreen-button" href="#">
          Fullscreen
        </a>
      </body>
      </html>
    `).window.document;

    // When
    exitFullscreen();

    // Then
    const fullscreenButton = document.getElementById('content-viewer_enter-fullscreen-button');
    expect(fullscreenButton!.classList).not.toContain('hidden');
  });

  it('should show the header', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <header class="nhsuk-header nhsuk-header--organisation nhsuk-header--white hidden" role="banner">
        </header>
      </body>
      </html>
    `).window.document;

    // When
    exitFullscreen();

    // Then
    const headerElement = document.getElementsByTagName('header')[0];
    expect(headerElement!.classList).not.toContain('hidden');
  });

  it('should show the footer', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <footer role="contentinfo" class="hidden">
        </footer>
      </body>
      </html>
    `).window.document;

    // When
    exitFullscreen();

    // Then
    const footer = document.getElementsByTagName('footer')[0];
    expect(footer!.classList).not.toContain('hidden');
  });

  it('should show the breadcrumbs', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <nav class="nhsuk-breadcrumb hidden" aria-label="Breadcrumb">
        </nav>
      </body>
      </html>
    `).window.document;

    // When
    exitFullscreen();

    // Then
    const breadcrumbs = document.getElementsByClassName('nhsuk-breadcrumb')[0];
    expect(breadcrumbs!.classList).not.toContain('hidden');
  });

  it('should make wrapper not fullscreen', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="nhsuk-u-margin-bottom-6 fullscreen" id="content-viewer_iframe-wrapper">
          <iframe
            src="https://www.dls.nhs.uk/CMS/CMSContent/Course324/Section1058/Tutorials/01-01-CreateaPresentation/itspplayer.html?CentreID=101&amp;CustomisationID=19068&amp;TutorialID=4671&amp;CandidateID=254480&amp;Version=2&amp;ProgressID=285049&amp;type=learn&amp;TrackURL=https://localhost:44367/tracking/tracker"
            class="nhsuk-u-margin-bottom-6 js-only-block content-viewer_iframe">
          </iframe>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    exitFullscreen();

    // Then
    const wrapper = document.getElementById('content-viewer_iframe-wrapper');
    expect(wrapper!.classList).not.toContain('fullscreen');
  });

  it('should add the wrapper class', () => {
    // Given
    global.document = new JSDOM(`
      <html>
      <head></head>
      <body>
        <div class="nhsuk-u-margin-bottom-6 fullscreen" id="content-viewer_iframe-wrapper">
          <iframe
            src="https://www.dls.nhs.uk/CMS/CMSContent/Course324/Section1058/Tutorials/01-01-CreateaPresentation/itspplayer.html?CentreID=101&amp;CustomisationID=19068&amp;TutorialID=4671&amp;CandidateID=254480&amp;Version=2&amp;ProgressID=285049&amp;type=learn&amp;TrackURL=https://localhost:44367/tracking/tracker"
            class="nhsuk-u-margin-bottom-6 js-only-block content-viewer_iframe">
          </iframe>
        </div>
      </body>
      </html>
    `).window.document;

    // When
    exitFullscreen();

    // Then
    const wrapper = document.getElementById('content-viewer_iframe-wrapper');
    expect(wrapper!.classList).toContain('content-viewer_iframe-wrapper');
  });
});
