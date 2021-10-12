import Chartist from 'chartist';
import getPathForEndpoint from '../common';

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

const options = {
  axisY: {
    scaleMinSpace: 10,
    onlyInteger: true,
  },
  chartPadding: {
    bottom: 32,
  },
};

const request = new XMLHttpRequest();
function processRequest() {
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
}

/*These constants are used in _ActivityTable.cshtml*/
const toggleableActivityButtonClass = "js-toggle-row-button";
const toggleableActivityRowClass = 'js-toggleable-activity-row';
// --

const path = getPathForEndpoint('TrackingSystem/Centre/Reports/Data');
const activityToggleableRowDisplayNone = 'none';
const activityToggleableRowDisplayTableRow = 'table-row';
const viewMoreLink =
  <HTMLElement>document.getElementsByClassName(toggleableActivityButtonClass).item(0);

function setUpToggleActivityRowsButton() {
  viewMoreLink.style.display = 'block';
}

viewMoreLink.addEventListener('click', (event) => {
  event.preventDefault();
  toggleVisibleActivityRows();
});

function toggleVisibleActivityRows() {
  const activityRow = <HTMLElement>document.getElementsByClassName(toggleableActivityRowClass).item(0);

  if (activityRow?.style.display === activityToggleableRowDisplayNone) {
    viewMoreRows();
    viewMoreLink.innerText = 'View Less';
  } else {
    viewLessRows();
    viewMoreLink.innerText = 'View More';
  }
}
function viewMoreRows(): void {
  const activityTableRows = <HTMLElement[]>Array.from(
    document.getElementsByClassName(toggleableActivityRowClass),
  );

  activityTableRows.forEach((row) => {
    const rowElement = row;
    rowElement.style.display = activityToggleableRowDisplayTableRow;
  });
}

function viewLessRows(): void {
  const activityTableRows = <HTMLElement[]>Array.from(
    document.getElementsByClassName(toggleableActivityRowClass),
  );

  activityTableRows.forEach((row) => {
    const rowElement = row;
    rowElement.style.display = activityToggleableRowDisplayNone;
  });
}

function pageLoad(){
  setUpToggleActivityRowsButton();
  viewLessRows();

  request.open('GET', path, true);
  request.responseType = 'json';
  request.send();
}

request.onload = () => {
  processRequest();
};

pageLoad();

