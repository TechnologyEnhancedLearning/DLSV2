document.addEventListener('DOMContentLoaded', () => {
  // Explicitly tell TypeScript these are HTMLInputElements
  const groups = document.querySelectorAll<HTMLDivElement>('.nhsuk-checkboxes');

  groups.forEach((group) => {
    // Cast the specific input types
    const groupToggle = group.querySelector<HTMLInputElement>('input[name="GroupIds"]');
    const childCheckboxes = group.querySelectorAll<HTMLInputElement>('input[name="SelectedCompetencyIds"]');

    if (!groupToggle) return;

    const syncChildren = () => {
      if (groupToggle.checked) {
        childCheckboxes.forEach(cb => cb.checked = true);
      }
    };

    syncChildren();

    groupToggle.addEventListener('change', () => {
      syncChildren();
      if (!groupToggle.checked) {
        childCheckboxes.forEach(cb => cb.checked = false);
      }
    });
  });
});
