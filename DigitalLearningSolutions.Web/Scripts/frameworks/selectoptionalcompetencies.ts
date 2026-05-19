document.addEventListener('DOMContentLoaded', () => {
  const groups = document.querySelectorAll<HTMLDivElement>('.nhsuk-checkboxes');

  groups.forEach((group) => {
    const groupToggle = group.querySelector<HTMLInputElement>('input[name="GroupIds"]');
    if (!groupToggle) return;

    // All individual competency checkboxes in the group
    const childCheckboxes = group.querySelectorAll<HTMLInputElement>(
      'input[name="SelectedCompetencyIds"]',
    );

    const updateState = () => {
      const isChecked = groupToggle.checked;

      childCheckboxes.forEach((_, index) => {
        childCheckboxes[index].checked = isChecked;
      });
    };

    // Run when the group checkbox changes
    groupToggle.addEventListener('change', updateState);

    // Also run at page load in case some are pre-checked server-side
    updateState();
  });
});
