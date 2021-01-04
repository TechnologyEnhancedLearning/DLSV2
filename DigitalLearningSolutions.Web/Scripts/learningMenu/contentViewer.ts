function closeMpe(): void {
  // Extract the current domain, customisationId, sectionId and tutorialId out of the URL
  const matches = window.location.href.match(/^(.*)\/LearningMenu\/(\d+)\/(\d+)\/(\d+)\/Tutorial$/);

  if (!matches || matches.length < 5) {
    return;
  }

  window.location.href = `${matches[1]}/LearningMenu/${matches[2]}/${matches[3]}/${matches[4]}`;
}

window.closeMpe = closeMpe;
