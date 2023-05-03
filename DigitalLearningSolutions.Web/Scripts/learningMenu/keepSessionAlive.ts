export const heartbeatInterval = 60000;

export function keepSessionAlive(): void {
  const request = new XMLHttpRequest();
  request.open('GET', '/keepSessionAlive/ping', true);
  request.send();
}
