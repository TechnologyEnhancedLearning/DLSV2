document.addEventListener('DOMContentLoaded', () => {
  const groups = document.querySelectorAll<HTMLDivElement>('.nhsuk-checkboxes');

  groups.forEach((group) => {
    const groupToggle =
      group.querySelector<HTMLInputElement>('input[name="GroupIds"]');

    const childCheckboxes =
      group.querySelectorAll<HTMLInputElement>('input[name="SelectedCompetencyIds"]');

    if (!groupToggle) {
      return;
    }

    const syncChildren = () => {
      if (groupToggle.checked) {
        childCheckboxes.forEach((checkbox) => {
          checkbox.checked = true;
        });
      }
    };

    syncChildren();

    groupToggle.addEventListener('change', () => {
      if (groupToggle.checked) {
        childCheckboxes.forEach((checkbox) => {
          checkbox.checked = true;
        });
      } else {
        childCheckboxes.forEach((checkbox) => {
          checkbox.checked = false;
        });
      }
    });
  });
});
