class CheckboxGroup {
  public static setUpSelectAndDeselectButtons(): void {
    const selectAllButtons = document.querySelectorAll('.select-all') as NodeListOf<HTMLAnchorElement>;
    selectAllButtons.forEach((button) => {
      button.addEventListener('click',
        () => {
          const group = button.getAttribute('data-group') as string;
          this.selectAll(group);
        });
    });

    const deselectAllButtons = document.querySelectorAll('.deselect-all') as NodeListOf<HTMLAnchorElement>;
    deselectAllButtons.forEach((button) => {
      button.addEventListener('click',
        () => {
          const group = button.getAttribute('data-group') as string;
          this.deselectAll(group);
        });
    });
  }

  static selectAll(group: string): void {
    const allCheckboxes = document.querySelectorAll('.select-all-checkbox') as NodeListOf<HTMLInputElement>;
    allCheckboxes.forEach((checkbox) => {
      if (checkbox.getAttribute('data-group') === group) {
        if (!checkbox.checked) checkbox.click();
      }
    });
  }

  static deselectAll(group: string): void {
    const allCheckboxes = document.querySelectorAll('.select-all-checkbox') as NodeListOf<HTMLInputElement>;
    allCheckboxes.forEach((checkbox) => {
      if (checkbox.getAttribute('data-group') === group) {
        if (checkbox.checked) checkbox.click();
      }
    });
  }
}

export default CheckboxGroup;
