import { SearchSortFilterAndPaginate } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

const progressId = (<HTMLSpanElement>document.getElementById('progress-id')).innerText.trim();

// eslint-disable-next-line no-new
new SearchSortFilterAndPaginate(
  `TrackingSystem/Delegates/ViewDelegate/DelegateProgress/${progressId}/AllLearningLogEntries`,
  false,
  false,
  false,
);
