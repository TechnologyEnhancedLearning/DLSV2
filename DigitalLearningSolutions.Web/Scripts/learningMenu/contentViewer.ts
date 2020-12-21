function closeMpe(): void {
  // Extract the customisationId, sectionId and tutorialId out of the URL
  const matches = window.location.href.match(/.*\/LearningMenu\/(\d+)\/(\d+)\/(\d+)\/Tutorial$/);

  if (!matches || matches.length < 4) {
    return;
  }

  window.location.href = `/LearningMenu/${matches[1]}/${matches[2]}/${matches[3]}`;
}

window.closeMpe = closeMpe;
