document.addEventListener('DOMContentLoaded', () => {
  const groups = document.querySelectorAll<HTMLDivElement>('.nhsuk-checkboxes');

  groups.forEach((group) => {
    const groupToggle =
      group.querySelector<HTMLInputElement>('input[name="GroupIds"]');
    // All individual competency checkboxes in the group
    const childCheckboxes =
      group.querySelectorAll<HTMLInputElement>(
        'input[name="SelectedCompetencyIds"]',
      );

    if (!groupToggle) {
      return;
    }

    const setChildrenCheckedState = (checked: boolean) => {
      childCheckboxes.forEach((checkboxElement) => {
        const checkbox = checkboxElement;
        checkbox.checked = checked;
      });
    };

    if (groupToggle.checked) {
      setChildrenCheckedState(true);
    }

    groupToggle.addEventListener('change', () => {
      setChildrenCheckedState(groupToggle.checked);
    });
  });
});
