export const heartbeatInterval = 60000;

export function keepSessionAlive(): void {
  const request = new XMLHttpRequest();
  const keepAlivePingPath = <HTMLInputElement>document.getElementById('keepAlivePingPath');
  const path = keepAlivePingPath?.value;
  request.open('GET', path, true);
  request.send();
}
