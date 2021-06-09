import React, { Component } from 'react';
import { UserLiveTime } from "./UserLiveTime";
import { RollingRetentionXDay } from "./RollingRetentionXDay";

export class BarChart extends Component {
  static displayName = BarChart.name;

  constructor(props) {
    super(props);
    this.rollingRetentionXDay = React.createRef();
    this.userLiveTime = React.createRef();
    this.state = {
      options: {
        scales: {
          yAxes: [
            {
              ticks: {
                beginAtZero: true,
              },
            },
          ],
        },
      },

      data: {}

    }
  }

  calculateRetentions = () => {
    this.rollingRetentionXDay.current.calculateRetentions();
    this.userLiveTime.current.calculateRetentions();
  }

  render() {
    return (<div className='p-5'>
      <div className='d-flex align-items-center justify-content-center mt-5'>
        <button
          type='button'
          className='btn btn-primary float-end'
          onClick={this.calculateRetentions}>
          Calculate Retentions
        </button>
      </div>

      <div className='p-5 pt-3'>
        <UserLiveTime ref={this.userLiveTime} />
      </div>
      <div className='p-5 pt-3'>
        <RollingRetentionXDay ref={this.rollingRetentionXDay} daysToCount={7} />
      </div>

    </div>)
  }
}