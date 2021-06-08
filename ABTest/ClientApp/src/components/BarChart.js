import React, { Component } from 'react';


export class BarChart extends Component {
  static displayName = BarChart.name;

  constructor(props) {
    super(props);
    let minDate = new Date();
    minDate.setDate(minDate.getDate() - 7);

    this.state = {
      chartData: [],

      yAxis: {
        minimum: 0,
        maximum: 25,
        interval: 2,
        title: "Live Users"
      },

      xAxis: {
        valueType: "DateTime",
        intervalType: "Days",
        interval: 1,
        minimum: minDate,
        maximum: new Date(),
        labelFormat: "dd/MM/yyyy",
        title: "Days",
      },
    }
  }

  getUserRetentions = () => {
    // get last 7 days user retentions
    fetch(`/api/users/GetUsersRollingRetentions/7`)
      .then((res) => { console.log(res); res.json() })
      .then((data) => {
        data.forEach(element => {
          element.day = new Date(element.day)
        });

        this.setState({
          chartData: data
        });

        console.log(this.state.chartData);
      });
  }

  render() {
    return (<div></div>)
    /*
    return (
      <div>
        
        <ChartComponent primaryXAxis={this.state.xAxis} primaryYAxis={this.state.yAxis} title="User Retentions last 7 days">
          <Inject services={[BarSeries, Legend, Tooltip, DataLabel, Category, DateTime]} />
          <SeriesCollectionDirective>
            <SeriesDirective dataSource={this.state.chartData} xName="day" yName="liveUsers" type="Bar">
            </SeriesDirective>
          </SeriesCollectionDirective>
        </ChartComponent>
        <button className="btn btn-primary mt-2" onClick={this.getUserRetentions}>Calculate</button>
      </div>
    );*/
  }
}