// Components
import Header from 'nhse-tel-frontend/packages/components/header/header';
import SkipLink from 'nhse-tel-frontend/packages/components/skip-link/skip-link';
import Details from 'nhse-tel-frontend/packages/components/details/details';
import Radios from 'nhse-tel-frontend/packages/components/radios/radios';
import Checkboxes from 'nhse-tel-frontend/packages/components/checkboxes/checkboxes';

// Polyfills
import 'nhse-tel-frontend/packages/polyfills';
import 'core-js/stable';
import 'regenerator-runtime/runtime';

// Initialize components
document.addEventListener(
  'DOMContentLoaded',
  () => {
    Details();
    Header();
    SkipLink();
    Radios();
    Checkboxes();
  },
);
