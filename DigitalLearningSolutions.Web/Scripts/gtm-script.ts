export { };
declare global {
  interface Window {
    push: any;
  }
  interface HTMLElement {
    src: any;
    async: any;
  }
}
(function (w: Window, d: Document, s: string, l: any, i: string) {
  const windowObj = w || window;
  windowObj[l] = windowObj[l] || [];
  windowObj[l].push({ 'gtm.start': new Date().getTime(), event: 'gtm.js' });
  var f = d.getElementsByTagName(s)[0],
    j = d.createElement(s),
    dl = l !== 'dataLayer' ? '&l=' + l : '';
  j.async = true;
  j.src = 'https://www.googletagmanager.com/gtm.js?id=' + i + dl;
  f.parentNode!.insertBefore(j, f);
})(window, document, 'script', 'dataLayer', 'GTM-KJPJHGW');
