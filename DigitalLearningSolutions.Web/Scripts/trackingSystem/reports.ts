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

function constructChartistData(rawData: any[]): any {
  rawData.reverse(); // TODO HEEDLS-458 remove the reversal in the backend and deal with it in the frontend where needed
  const labels = rawData.map(d => d.month);
  const series = [
    rawData.map(d => d.completions),
    rawData.map(d => d.evaluations),
    rawData.map(d => d.registrations)
  ];
  return { labels, series };
}

const path = getPathForEndpoint("TrackingSystem/Centre/Reports/Data");


const request = new XMLHttpRequest();

const options = {
  axisY: {
    onlyInteger: true
  },
  plugins: [
    Chartist.plugins.ctAxisTitle({
      axisX: {
        axisTitle: 'Month'
      },
      axisY: {
        axisTitle: '#'
      }
    })
  ]
}

request.onload = () => {
  console.log(request.response);
  var foo = new Chartist.Line('.ct-chart', constructChartistData(request.response), options);
};

request.open('GET', path, true);
request.responseType = 'json';
request.send();


//requestPromise.then((response) => {
//  if (response === null) {
//    return undefined;
//  }
//
//  new Chartist.Line('.ct-chart', data);
//});

// Create a new line chart object where as first parameter we pass in a selector
// that is resolving to our chart container element. The Second parameter
// is the actual data object.
