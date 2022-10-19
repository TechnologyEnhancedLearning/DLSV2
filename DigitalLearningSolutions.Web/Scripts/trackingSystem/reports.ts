import Chartist, { IChartistData } from 'chartist';
import getPathForEndpoint from '../common';

// These constants are used in _ActivityTable.cshtml
const toggleableActivityButtonId = 'js-toggle-row-button';
const toggleableActivityRowClass = 'js-toggleable-activity-row';

// These constants are used in /Views/TrackingSystem/Centre/Reports/Index.cshtml
const activityGraphId = 'activity-graph';
const activityGraphDataErrorId = 'activity-graph-data-error';

const path = getPathForEndpoint('TrackingSystem/Centre/Reports/Courses/Data');
const activityToggleableRowDisplayNone = 'none';
const activityToggleableRowDisplayTableRow = 'table-row';

const mobileMaxNumberOfEntriesForActivityGraph = 17;
const desktopMaxNumberOfEntriesForActivityGraph = 31;

const noActivityMessageId = 'no-activity-message';
const noActivityMessage = <HTMLElement>document.getElementById(noActivityMessageId);

let chartData: IChartistData;

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

function saveChartData(request: XMLHttpRequest) {
  let { response } = request;
  // IE does not support automatic parsing to JSON with XMLHttpRequest.responseType
  // so we need to manually parse the JSON string if not already parsed
  if (typeof request.response === 'string') {
    response = JSON.parse(response);
  }
  chartData = constructChartistData(response);
}

function drawChartOrDataPointMessage() {
  const numberOfEntries = chartData.labels?.length;
  const mediaQuery = window.matchMedia('(min-width: 641px)');
  const maxNumberOfEntriesForGraph = mediaQuery.matches
    ? desktopMaxNumberOfEntriesForActivityGraph
    : mobileMaxNumberOfEntriesForActivityGraph;

  if (numberOfEntries !== undefined && numberOfEntries <= maxNumberOfEntriesForGraph) {
    drawChart();
  } else {
    displayTooManyDataPointsMessage();
  }
}

function drawChart() {
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

  const chart = new Chartist.Line('.ct-chart', chartData, options);

  chart.on(
    'draw',
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
        const labelTextLength = element.getNode().textContent.length;
        // this should match the NHS tablet breakpoint
        const mediaQuery = window.matchMedia('(min-width: 641px)');
        const rotation = mediaQuery.matches ? -45 : -60;
        // Since we're rotating the elements, we need to shift them slightly left and down a
        // variable amount based on the length of the label being displayed. The following
        // formulae set up this adjustment. We include the translateX as a modifier in
        // the rotation so that the labels will still come out at the correct angle.
        const translateX = labelTextLength * -5;
        const translateY = labelTextLength - 4;
        element.attr({
          transform: `translate(${translateX} ${translateY}) rotate(${rotation} ${xOrigin - translateX} ${yOrigin})`,
        });
      }
    },
  );

  const dataPointErrorContainer = getActivityGraphDataErrorElement();
  if (dataPointErrorContainer !== null) {
    dataPointErrorContainer.hidden = true;
    dataPointErrorContainer.style.display = 'none';
  }
  const chartContainer = getActivityGraphElement();
  if (chartContainer !== null) {
    chartContainer.hidden = false;
    chartContainer.style.display = 'block';
  }
}

function displayTooManyDataPointsMessage() {
  const dataPointErrorContainer = getActivityGraphDataErrorElement();
  if (dataPointErrorContainer !== null) {
    dataPointErrorContainer.hidden = false;
    dataPointErrorContainer.style.display = 'block';
  }
  const chartContainer = getActivityGraphElement();
  if (chartContainer !== null) {
    chartContainer.hidden = true;
    chartContainer.style.display = 'none';
  }
}

function getActivityGraphElement(): HTMLElement | null {
  return document.getElementById(activityGraphId);
}

function getActivityGraphDataErrorElement(): HTMLElement | null {
  return document.getElementById(activityGraphDataErrorId);
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

function fetchChartDataAndDrawGraph() {
  const request = new XMLHttpRequest();

  request.onload = () => {
    saveChartData(request);
    drawChartOrDataPointMessage();
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

if (!noActivityMessage) {
  setUpToggleActivityRowsButton();
  viewLessRows();
  fetchChartDataAndDrawGraph();
  window.onresize = drawChartOrDataPointMessage;
}
