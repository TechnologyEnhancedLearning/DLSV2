import Chartist from 'chartist';
import 'chartist-plugin-axistitle';

var data = {
  // A labels array that can contain any sort of values
  labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'],
  // Our series array that contains series objects or in this case series data arrays
  series: [
    [5, 2, 4, 2, 0]
  ]
};

function getPathForEndpoint(endpoint: string): string { // TODO HEEDLS-458 from searchSortAndPaginate, commonise
  const currentPath = window.location.pathname;
  const endpointUrlParts = endpoint.split('/');
  const indexOfBaseUrl = currentPath.indexOf(endpointUrlParts[0]);
  return `${currentPath.substring(0, indexOfBaseUrl)}${endpoint}`;
}

function constructChartistData(model: any): any { // TODO HEEDLS-458 this should have defined types for parameters and return values
  const data = model.rows;
  data.reverse(); // TODO HEEDLS-458 remove the reversal in the backend and deal with it in the frontend where needed
  const labels = data.map((d: any) => d.period);
  const series = [
    data.map((d: any) => d.completions),
    data.map((d: any) => d.evaluations),
    data.map((d: any) => d.registrations)
  ];
  return { labels, series };
}

const path = getPathForEndpoint("TrackingSystem/Centre/Reports/Data");

const request = new XMLHttpRequest();

const options = {
  axisY: {
    onlyInteger: true
  },
  chartPadding: { bottom: 32 }
}

request.onload = () => {
  new Chartist.Line('.ct-chart', constructChartistData(request.response), options);
};

request.open('GET', path, true);
request.responseType = 'json';
request.send();
