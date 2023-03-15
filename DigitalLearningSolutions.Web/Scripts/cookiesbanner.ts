var divCookieBanner = document.getElementById('cookiebanner');
var bannerConfirm = document.getElementById('nhsuk-cookie-confirmation-banner');
var bannerCookieAccept = document.getElementById('nhsuk-cookie-banner__link_accept_analytics');
var bannerCookieReject = document.getElementById('nhsuk-cookie-banner__link_accept');
var divCookieBannerNoJSstyling = document.getElementById('cookie-banner-no-js-styling');
var divCookieBannerJSstyling = document.getElementById('cookie-banner-js-styling');

if (divCookieBannerNoJSstyling != null) {
  divCookieBannerNoJSstyling.setAttribute("style", "display:none;");
}
if (divCookieBannerJSstyling != null) {
  divCookieBannerJSstyling.setAttribute("style", "display:block;");
}

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
  var path = '/LearningSolutions/CookieBannerConfirmation'

  request.open('GET', path + '?' + params, true);
  request.setRequestHeader('Content-type', 'application/x-www-form-urlencoded;charset=UTF-8');
  request.send();
};

var consentYes = document.getElementById("input-statistics-1");
var consentNo = document.getElementById("input-statistics-2");

if (consentYes != null) {
  consentYes.addEventListener('click', function () {
    return setUserConsent('true');
  });
}

if (consentNo != null) {
  consentNo.addEventListener('click', function () {
    return setUserConsent('false');
  });
}
function setUserConsent(value: string) {
  var userConsent = <HTMLInputElement>document.getElementById("UserConsent"); 
  if (userConsent != null) {
    userConsent.value = value;
  }  
}
