import Chartist from 'chartist';
import 'chartist-plugin-axistitle';
import { getPathForEndpoint } from '../common';

interface IActivityDataRowModel {
  period: string;
  completions: number;
  evaluations: number;
  registrations: number;
}

function constructChartistData(data: Array<IActivityDataRowModel>): Chartist.IChartistData {
  const labels = data.map((d: any) => d.period);
  const series = [
    data.map((d: any) => d.completions),
    data.map((d: any) => d.evaluations),
    data.map((d: any) => d.registrations),
  ];
  return { labels, series };
}

const path = getPathForEndpoint('TrackingSystem/Centre/Reports/Data');

const request = new XMLHttpRequest();

const options = {
  axisY: {
    onlyInteger: true,
  },
  chartPadding: { bottom: 32 },
};

request.onload = () => {
  const data = constructChartistData(request.response);
  // eslint-disable-next-line no-new
  new Chartist.Line('.ct-chart', data, options);
};

request.open('GET', path, true);
request.responseType = 'json';
request.send();
