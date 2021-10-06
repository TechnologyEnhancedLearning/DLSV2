import Chartist from 'chartist';
import getPathForEndpoint from '../common';

showMoreRows(false);

interface IActivityDataRowModel {
  period: string;
  completions: number;
  evaluations: number;
  registrations: number;
}

function constructChartistData(data: Array<IActivityDataRowModel>): Chartist.IChartistData {
  const labels = data.map((d) => d.period);
  const series = [
    data.map((d) => d.completions),
    data.map((d) => d.evaluations),
    data.map((d) => d.registrations),
  ];
  return { labels, series };
}

const path = getPathForEndpoint('TrackingSystem/Centre/Reports/Data');

const request = new XMLHttpRequest();

const options = {
  axisY: {
    scaleMinSpace: 10,
    onlyInteger: true,
  },
  chartPadding: {
    bottom: 32,
  },
};

request.onload = () => {
  let { response } = request;
  // IE does not support automatic parsing to JSON with XMLHttpRequest.responseType
  // so we need to manually parse the JSON string if not already parsed
  if (typeof request.response === 'string') {
    response = JSON.parse(response);
  }
  const data = constructChartistData(response);
  const chart = new Chartist.Line('.ct-chart', data, options);

  chart.on('draw',
    // The type here is Chartist.ChartDrawData, but the type specification is missing getNode()
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    (drawnElement: any) => {
      const { element } = drawnElement;
      // IE renders text in SVGs with 'text' tags that do not work with most CSS properties
      // so we set the relevant attributes manually
      if (element.getNode().tagName === 'text'
        && element.classes().indexOf('ct-horizontal') >= 0
        && element.classes().indexOf('ct-label') >= 0) {
        const xOrigin = Number(element.getNode().getAttribute('x'));
        const yOrigin = element.getNode().getAttribute('y');
        const width = Number(element.getNode().getAttribute('width'));
        // this should match the NHS tablet breakpoint
        const mediaQuery = window.matchMedia('(min-width: 641px)');
        const rotation = mediaQuery.matches ? -45 : -60;
        element.attr({
          transform: `translate(-${width}) rotate(${rotation} ${xOrigin + width} ${yOrigin})`,
        });
      }
    });
};

request.open('GET', path, true);
request.responseType = 'json';
request.send();

let allRowsShown = false;
const viewMoreLink = <HTMLLIElement>document.getElementsByClassName('js-shown-load-more').item(0);
viewMoreLink.style.display = 'block';

viewMoreLink.addEventListener('click', (event) => {
  event.preventDefault();
  allRowsShown = !allRowsShown;
  showMoreRows(allRowsShown);
  viewMoreLink.innerText = allRowsShown ? 'View Less' : 'View More';
});

function showMoreRows(status: boolean): void {
  const activityTableRows = <HTMLElement[]>Array.from(
    document.getElementsByClassName('js-hidden-activity-row'),
  );

  activityTableRows.forEach((row) => {
    const rowElement = row;
    rowElement.style.display = (status ? 'table-row' : 'none');
  });
}
