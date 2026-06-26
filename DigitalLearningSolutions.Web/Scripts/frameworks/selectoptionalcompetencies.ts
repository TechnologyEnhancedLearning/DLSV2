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

      childCheckboxes.forEach((cb) => {
        if (isChecked) {
          // eslint-disable-next-line no-param-reassign
          cb.checked = true; // force selected
          // eslint-disable-next-line no-param-reassign
          cb.disabled = true; // lock them
        } else {
          // eslint-disable-next-line no-param-reassign
          cb.disabled = false; // re-enable when group is unchecked
          // optional: leave cb.checked unchanged
        }
      });
    };

    // Run when the group checkbox changes
    groupToggle.addEventListener('change', updateState);

    // Also run at page load in case some are pre-checked server-side
    updateState();
  });
});
