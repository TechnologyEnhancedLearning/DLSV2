function closeMpe(): void {
  // Extract the customisationId and sectionId out of the URL
  const matches = window.location.href.match(/.*\/LearningMenu\/(\d+)\/(\d+)\/(\d+)\/Tutorial$/);

  if (matches == null || matches.length < 3) {
    return;
  }

  window.location.href = `/LearningMenu/${matches[1]}/${matches[2]}`;
}

window.closeMpe = closeMpe;
// export default closeMpe;
