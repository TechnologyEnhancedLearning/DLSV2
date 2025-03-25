async function setTableauParams(): Promise<void> {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const viz = document.getElementById('tableau-viz') as any;
  const adminIdElement = document.getElementById('hf-adminid') as HTMLInputElement | null;
  const emailElement = document.getElementById('hf-email') as HTMLInputElement | null;

  if (!viz) {
    // console.error('Tableau Viz element not found.');
    return;
  }

  if (!adminIdElement || !emailElement) {
    // console.error('Hidden input fields for adminid or email not found.');
    return;
  }

  const adminId = adminIdElement.value;
  const email = emailElement.value;

  // Listen for the `firstinteractive` event from the Web Component
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  viz.addEventListener('firstinteractive', async (event: Event) => {
    await viz.workbook.changeParameterValueAsync('adminid', adminId);
    await viz.workbook.changeParameterValueAsync('email', email);
  });
}

// Run the function after the page loads
document.addEventListener('DOMContentLoaded', () => {
  setTableauParams();
});
