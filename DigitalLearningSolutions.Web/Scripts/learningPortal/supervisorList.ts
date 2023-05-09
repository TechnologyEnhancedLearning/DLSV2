import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';
import { setItemsPerPage } from '../searchSortFilterAndPaginate/paginate';

setItemsPerPage(9999);
const selfAssessment = <HTMLInputElement>document.getElementById('SelfAssessmentID');
const selfAssessmentId = selfAssessment.value;
// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`LearningPortal/SelfAssessment/${selfAssessmentId}/GetAllSupervisors`, true, true, false);

const sInput = <HTMLSelectElement>document.getElementById('search-field');
if (sInput != null) {
  sInput.addEventListener('onpaste', handler, false);
  sInput.addEventListener('oncut', handler, false);
  sInput.addEventListener('keyup', handler, false);
}
function handler(event:any) {
  const sp = <HTMLSelectElement>document.getElementById('result-count-and-page-number');
  const btnSubmit = <HTMLSelectElement>document.getElementById('btnAddSupervisor');
  if (sp.innerText.startsWith('0 matching')) {
    btnSubmit.style.visibility = 'hidden';
  } else {
    btnSubmit.style.visibility = 'visible';
  }
}
