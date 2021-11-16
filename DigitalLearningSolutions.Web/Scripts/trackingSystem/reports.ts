import Chartist from 'chartist';
import getPathForEndpoint from '../common';

// These constants are used in _ActivityTable.cshtml
const toggleableActivityButtonId = 'js-toggle-row-button';
const toggleableActivityRowClass = 'js-toggleable-activity-row';

const path = getPathForEndpoint('TrackingSystem/Centre/Reports/Data');
const activityToggleableRowDisplayNone = 'none';
const activityToggleableRowDisplayTableRow = 'table-row';

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

function generateChart(request: XMLHttpRequest) {
  let { response } = request;
  // IE does not support automatic parsing to JSON with XMLHttpRequest.responseType
  // so we need to manually parse the JSON string if not already parsed
  if (typeof request.response === 'string') {
    response = JSON.parse(response);
  }
  const data = constructChartistData(response);

  const options = {
    fullWidth: true,
    axisY: {
      scaleMinSpace: 10,
      onlyInteger: true,
    },
    chartPadding: {
      bottom: 32,
    },
  };

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

function toggleVisibleActivityRows() {
  const viewMoreLink = getViewMoreLink();
  const activityRow = <HTMLElement>document.getElementsByClassName(toggleableActivityRowClass)
    .item(0);

  if (activityRow?.style.display === activityToggleableRowDisplayNone) {
    viewMoreRows();
    viewMoreLink.innerText = 'View less';
  } else {
    viewLessRows();
    viewMoreLink.innerText = 'View more';
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

function makeChartistRequest() {
  const request = new XMLHttpRequest();

  request.onload = () => {
    generateChart(request);
  };

  request.open('GET', path, true);
  request.responseType = 'json';
  request.send();
}

function getViewMoreLink() {
  return <HTMLElement>document.getElementById(toggleableActivityButtonId);
}

function setUpToggleActivityRowsButton() {
  const viewMoreLink = getViewMoreLink();

  viewMoreLink.addEventListener('click', (event) => {
    event.preventDefault();
    toggleVisibleActivityRows();
  });
}

setUpToggleActivityRowsButton();
viewLessRows();
makeChartistRequest();
