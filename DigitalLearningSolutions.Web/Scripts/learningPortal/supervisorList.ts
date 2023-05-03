import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const selfAssessment = <HTMLInputElement>document.getElementById('SelfAssessmentID');
const selfAssessmentId = selfAssessment.value;

new SearchSortFilterAndPaginate(`LearningPortal/SelfAssessment/${selfAssessmentId}/GetAllSupervisors`, true, true, false);


const sInput = <HTMLSelectElement>document.getElementById('search-field');
if (sInput != null) {
  sInput.addEventListener('change', spanChanged);
}
function spanChanged() {
  const sp = <HTMLSelectElement>document.getElementById('result-count-and-page-number');

  const btnSubmit = <HTMLSelectElement>document.getElementById('btnAddSupervisor');

  if (sp.innerText.startsWith('0 matching')) {

    btnSubmit.focus();
    btnSubmit.disabled = true;
  }
  else {
    btnSubmit.disabled = false;
  }
}


