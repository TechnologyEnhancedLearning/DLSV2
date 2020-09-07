/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { CourseCard } from '../learningPortal/searchSortAndPaginate';

export default function getCourseCards(): CourseCard[] {
  return Array.from(document.getElementById('course-cards')!.children).map((card) => ({
    title: card.getElementsByClassName('course-title')[0].textContent,
    element: card,
  } as CourseCard));
}
