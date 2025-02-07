// Components
import Header from '@technology-enhanced-learning/nhse-tel-frontend/packages/components/header/header';
import SkipLink from '@technology-enhanced-learning/nhse-tel-frontend/packages/components/skip-link/skip-link';
import Details from '@technology-enhanced-learning/nhse-tel-frontend/packages/components/details/details';
import Radios from '@technology-enhanced-learning/nhse-tel-frontend/packages/components/radios/radios';
import Checkboxes from '@technology-enhanced-learning/nhse-tel-frontend/packages/components/checkboxes/checkboxes';

// Polyfills
import '@technology-enhanced-learning/nhse-tel-frontend/packages/polyfills';
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
