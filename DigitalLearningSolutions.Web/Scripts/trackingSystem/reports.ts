import Chartist, { ChartDrawData } from 'chartist';
import getPathForEndpoint from '../common';

interface IActivityDataRowModel {
  period: string;
  completions: number;
  evaluations: number;
  registrations: number;
}

interface IEvaluationSummaryDataModel {
  id: string;
  responseCounts: Array<IResponseCount> | null
}

interface IResponseCount {
  name: string;
  count: number;
}

function setUpGraphs(): void {
  const path = getPathForEndpoint('TrackingSystem/Centre/Reports/Data');

  const request = new XMLHttpRequest();

  request.onload = () => {
    let { response } = request;
    // IE does not support automatic parsing to JSON with XMLHttpRequest.responseType
    // so we need to manually parse the JSON string if not already parsed
    if (typeof request.response === 'string') {
      response = JSON.parse(response);
    }

    setUpActivityGraph(response.activityGraphData);
    setUpEvaluationSummaryCharts(response.evaluationSummariesData);
  };

  request.open('GET', path, true);
  request.responseType = 'json';
  request.send();
}

function setUpActivityGraph(inputData: Array<IActivityDataRowModel>): void {
  const options = {
    axisY: {
      scaleMinSpace: 10,
      onlyInteger: true,
    },
    chartPadding: {
      bottom: 32,
    },
  };

  const data = constructActivityGraphChartistData(inputData);
  const chart = new Chartist.Line('#activity-graph', data, options);

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

function constructActivityGraphChartistData(data: Array<IActivityDataRowModel>):
  Chartist.IChartistData {
  const labels = data.map((d) => d.period);
  const series = [
    data.map((d) => d.completions),
    data.map((d) => d.evaluations),
    data.map((d) => d.registrations),
  ];
  return { labels, series };
}

function sum(a: number, b: number): number {
  return a + b;
}

function setUpEvaluationSummaryCharts(inputData: Array<IEvaluationSummaryDataModel>): void {
  inputData.forEach((data) => setUpEvaluationSummaryChart(data));
}

function setUpEvaluationSummaryChart(inputData: IEvaluationSummaryDataModel): void {
  if (inputData.responseCounts) {
    const data = constructEvaluationSummaryChartData(inputData.responseCounts);
    const totalCount = inputData.responseCounts.map((rc) => rc.count).reduce(sum);

    const options = {
      chartPadding: 32,
      height: 200,
      donut: true,
      donutWidth: 50,
      donutSolid: true,
      startAngle: 270,
      showLabel: true,
      labelOffset: 20,
      labelPosition: 'outside',
      labelInterpolationFnc(value: number) {
        return value !== 0 ? `${((value / totalCount) * 100).toFixed(1)}%` : '';
      },
    };
    const chart = new Chartist.Pie(`#${inputData.id}`, data, options);

    chart.on('draw',
      (drawnElement: ChartDrawData) => {
        if (drawnElement.type === 'label') {
          //drawnElement.element.attr({
          //  style: 'stroke: yellow;',
          //});
        }
      });
  }
}

function constructEvaluationSummaryChartData(responseCounts: Array<IResponseCount>):
  Chartist.IChartistData {
  const series = responseCounts.map((rc) => ({ value: rc.count, className: `ct-series-${rc.name}` }));
  return { series };
}

setUpGraphs();
