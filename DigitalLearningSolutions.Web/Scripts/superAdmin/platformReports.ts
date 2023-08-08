import Chartist, { IChartistData } from 'chartist';
import getPathForEndpoint from '../common';
import * as chartCommon from '../chartCommon';
const pagePath = window.location.pathname;
const path = getPathForEndpoint(pagePath.concat('/Data'));
interface IActivityDataRowModel {
  period: string;
  completions: number;
  enrolments: number;
}

function constructChartistData(data: Array<IActivityDataRowModel>): Chartist.IChartistData {
  const labels = data.map((d) => d.period);
  const series = [
    data.map((d) => d.completions),
    data.map((d) => d.enrolments),
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
  const data = constructChartistData(response);
  chartCommon.chartData.labels = data.labels;
  chartCommon.chartData.series = data.series;
}

function fetchChartDataAndDrawGraph() {
  const request = new XMLHttpRequest();

  request.onload = () => {
    saveChartData(request);
    chartCommon.drawChartOrDataPointMessage();
  };

  request.open('GET', path, true);
  request.responseType = 'json';
  request.send();
}

if (!chartCommon.noActivityMessage) {
  chartCommon.setUpToggleActivityRowsButton();
  chartCommon.viewLessRows();
  fetchChartDataAndDrawGraph();
  window.onresize = chartCommon.drawChartOrDataPointMessage;
}
