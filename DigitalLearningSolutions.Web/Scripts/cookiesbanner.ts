const divCookieBanner = document.getElementById('cookiebanner');
const divCookieBannerNoJSstyling = document.getElementById('cookie-banner-no-js-styling');
const divCookieBannerJSstyling = document.getElementById('cookie-banner-js-styling');
const bannerConfirm = document.getElementById('nhsuk-cookie-confirmation-banner');
const bannerConfirmOnPost = document.getElementById('nhsuk-cookie-confirmation-banner-post');
const bannerCookieAccept = document.getElementById('nhsuk-cookie-banner__link_accept_analytics');
const bannerCookieReject = document.getElementById('nhsuk-cookie-banner__link_accept');

const path = '/CookieConsent/ConfirmCookieConsent';

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

/*[BY] Handled the following in MVC Post**************************/
/*
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
}*/
