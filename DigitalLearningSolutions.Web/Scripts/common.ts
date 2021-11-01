export default function getPathForEndpoint(endpoint: string): string {
  const currentPath = window.location.pathname;
  const endpointUrlParts = endpoint.split('/');
  const indexOfBaseUrl = currentPath.indexOf(endpointUrlParts[0]);
  return `${currentPath.substring(0, indexOfBaseUrl)}${endpoint}`;
}

/** This allows us to dispatch browser events in old IE and newer browsers */
export function sendBrowserAgnosticEvent <T extends HTMLElement>(
  elem: T,
  eventName: string,
): Event {
  // Needed for IE Support: https://developer.mozilla.org/en-US/docs/Web/API/EventTarget/dispatchEvent#Browser_Compatibility
  // https://stackoverflow.com/a/49071358/79677
  let event;
  if (typeof (Event) === 'function') {
    event = new Event(eventName);
  } else {
    event = document.createEvent('Event');
    event.initEvent(eventName, true, true);
  }
  elem.dispatchEvent(event);

  return event;
}

export function selectAll(selectorClass: string): void {
  const allCheckboxes = document.querySelectorAll(selectorClass) as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => {
    if (!checkbox.checked) checkbox.click();
  });
}

export function deselectAll(selectorClass: string): void {
  const allCheckboxes = document.querySelectorAll(selectorClass) as NodeListOf<HTMLInputElement>;
  allCheckboxes.forEach((checkbox) => {
    if (checkbox.checked) checkbox.click();
  });
}
