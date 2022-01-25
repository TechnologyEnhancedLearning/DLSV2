import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const selfAssessment = <HTMLSpanElement>document.getElementById('selfAssessmentID');
const selfAssessmentId = selfAssessment?.innerHTML.trim();
// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(`LearningPortal/SelfAssessment/${selfAssessmentId}/AllRecommendedLearningItems`, true, true, false);
