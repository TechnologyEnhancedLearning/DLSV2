class Checkboxes {
  public static setUpSelectAndDeselectInGroupButtons(): void {
    const selectAllButtons = document.querySelectorAll('.select-all') as NodeListOf<HTMLAnchorElement>;
    selectAllButtons.forEach((button) => {
      button.addEventListener('click',
        () => {
          const group = button.getAttribute('data-group') as string;
          this.selectAllInGroup(group);
        });
    });

    const deselectAllButtons = document.querySelectorAll('.deselect-all') as NodeListOf<HTMLAnchorElement>;
    deselectAllButtons.forEach((button) => {
      button.addEventListener('click',
        () => {
          const group = button.getAttribute('data-group') as string;
          this.deselectAllinGroup(group);
        });
    });
  }

  static selectAllInGroup(group: string): void {
    const allCheckboxes = document.querySelectorAll('.select-all-checkbox') as NodeListOf<HTMLInputElement>;
    allCheckboxes.forEach((checkbox) => {
      if (checkbox.getAttribute('data-group') === group) {
        if (!checkbox.checked) checkbox.click();
      }
    });
  }

  static deselectAllinGroup(group: string): void {
    const allCheckboxes = document.querySelectorAll('.select-all-checkbox') as NodeListOf<HTMLInputElement>;
    allCheckboxes.forEach((checkbox) => {
      if (checkbox.getAttribute('data-group') === group) {
        if (checkbox.checked) checkbox.click();
      }
    });
  }

  static selectAll(selectorClass: string): void {
    const allCheckboxes = document.querySelectorAll(selectorClass) as NodeListOf<HTMLInputElement>;
    allCheckboxes.forEach((checkbox) => {
      if (!checkbox.checked) checkbox.click();
    });
  }

  static deselectAll(selectorClass: string): void {
    const allCheckboxes = document.querySelectorAll(selectorClass) as NodeListOf<HTMLInputElement>;
    allCheckboxes.forEach((checkbox) => {
      if (checkbox.checked) checkbox.click();
    });
  }
}

export default Checkboxes;
