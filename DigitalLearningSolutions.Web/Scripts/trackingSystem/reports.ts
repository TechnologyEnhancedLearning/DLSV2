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
  let response = request.response;
  // IE11 does not support automatic parsing to JSON with XMLHttpRequest.responseType, so we need to manually parse the JSON string
  if (typeof request.response === 'string') {
    response = JSON.parse(response);
  }
  const data = constructChartistData(response);
  // eslint-disable-next-line no-new
  const chart = new Chartist.Line('.ct-chart', data, options);

  chart.on('draw',
    (foo: any) => {
      const element = foo.element;
      if (element.classes().indexOf('ct-horizontal') >= 0 && element.classes().indexOf('ct-label') >= 0) {
        const xOrigin = element.getNode().getAttribute("x");
        const yOrigin = element.getNode().getAttribute("y");
        const width = element.getNode().getAttribute("width");
        const height = element.getNode().getAttribute("height");
          element.attr({
          transform: `translate(-${width/2} ${height}) rotate(-45 ${xOrigin} ${yOrigin})`
        });
      }
    });
};

request.open('GET', path, true);
request.responseType = 'json';
request.send();
