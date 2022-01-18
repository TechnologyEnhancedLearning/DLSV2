/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { ISearchableElement } from '../searchSortFilterAndPaginate/searchSortFilterAndPaginate';

export default function getSearchableElements(): ISearchableElement[] {
  return Array.from(document.getElementById('searchable-elements')!.children).map((searchableElement) => ({
    searchableContent: searchableElement.getElementsByClassName('searchable-element-title')[0].textContent,
    element: searchableElement,
  } as ISearchableElement));
}
