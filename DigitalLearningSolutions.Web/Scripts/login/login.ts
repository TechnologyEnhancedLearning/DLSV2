const timeZone: string = Intl.DateTimeFormat().resolvedOptions().timeZone;
const timeZoneElement = document.getElementById("timeZone") as HTMLInputElement;
if (timeZoneElement) {
  timeZoneElement.value = timeZone;
}
