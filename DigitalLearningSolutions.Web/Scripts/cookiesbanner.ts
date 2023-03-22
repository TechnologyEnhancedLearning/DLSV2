const divCookieBanner = document.getElementById('cookiebanner');
const divCookieBannerNoJSstyling = document.getElementById('cookie-banner-no-js-styling');
const divCookieBannerJSstyling = document.getElementById('cookie-banner-js-styling');
const bannerConfirm = document.getElementById('nhsuk-cookie-confirmation-banner');
const bannerConfirmOnPost = document.getElementById('nhsuk-cookie-confirmation-banner-post');
const bannerCookieAccept = document.getElementById('nhsuk-cookie-banner__link_accept_analytics');
const bannerCookieReject = document.getElementById('nhsuk-cookie-banner__link_accept');

const path = document.getElementById('CookieConsentPostPath');
//const path = getFullURL('/CookieConsent/ConfirmCookieConsent');

if (divCookieBannerNoJSstyling != null) {
  divCookieBannerNoJSstyling.setAttribute("style", "display:none;");
}
if (divCookieBannerJSstyling != null) {
  divCookieBannerJSstyling.setAttribute("style", "display:block;");  
}
bannerConfirmOnPost?.setAttribute("style", "display:none;");

if (bannerCookieAccept != null) {
  bannerCookieAccept.addEventListener('click', function () {
    return bannerAccept("true");
  });
}

if (bannerCookieReject != null) {
  bannerCookieReject.addEventListener('click', function () {
    return bannerAccept("false");
  });
}

function bannerAccept(consentValue: string) {
  if (divCookieBanner != null) {
    divCookieBanner.setAttribute("style", "display:none;");
  }

  if (bannerConfirm != null) {
    bannerConfirm.setAttribute("style", "display:block;");
  }
  changeConsent(consentValue);
}

function changeConsent(consent: string) {
  var params = 'consent=' + consent;
  var request = new XMLHttpRequest();  

  request.open('GET', path + '?' + params, true);
  request.setRequestHeader('Content-type', 'application/x-www-form-urlencoded;charset=UTF-8');
  request.send();
};


function getFullURL(endpoint: string): string {
  const currentHref = window.location.href;
  const currentPath = window.location.pathname;
  const fullDomain = currentHref.replace(currentPath, "");
  return `${fullDomain}${endpoint}`;
}
