/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { SearchableElement } from '../searchSortAndPaginate/searchSortAndPaginate';

export default function getSearchableElements(): SearchableElement[] {
  return Array.from(document.getElementById('searchable-elements')!.children).map((searchableElement) => ({
    title: searchableElement.getElementsByClassName('searchable-element-title')[0].textContent,
    element: searchableElement,
  } as SearchableElement));
}
