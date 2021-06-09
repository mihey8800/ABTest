import React, { Component } from "react";
import { Bar } from 'react-chartjs-2';
import { formattedDate } from '../helpers/formattedDate'

export class RollingRetentionXDay extends Component {

    constructor(props) {
        super(props);
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
            data: []

        }
        this.calculateRetentions = this.calculateRetentions.bind(this);
    }



    calculateRetentions = () => {
        // get last 7 days user retentions
        fetch(`/api/users/GetUsersRollingRetentionsXDay/${this.props.daysToCount}`)
            .then((res) => {
                res.json().then((data) => {
                    data.forEach(element => {
                        element.day = formattedDate(new Date(element.day))
                    });

                    var result = {
                        labels: data.map((data) => data.day),
                        datasets: [
                            {
                                label: "Percent",
                                data: data.map((data) => data.percent),
                                backgroundColor: "rgba(54, 162, 235, 0.2)",
                                borderColor: "rgba(54, 162, 235, 1)",
                                borderWidth: 1,
                            },
                        ],
                    };

                    this.setState({ data: result });
                });
            })

    }

    render() {
        return (
            <div>
                <h2 className='text-secondary'>Rolling retention {this.props.daysToCount} day</h2>
                <Bar data={this.state.data} options={this.state.options} />
            </div>
        );
    }
}
