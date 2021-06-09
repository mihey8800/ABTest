import React, { Component } from "react";
import { Line } from 'react-chartjs-2';

export class UserLiveTime extends Component {

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
        fetch(`/api/users/GetUsersUsersLiveTime`)
            .then((res) => {
                res.json().then((data) => {

                    var result = {
                        labels: data.map((data) => "User " + data.user.id),
                        datasets: [
                            {
                                label: "Lifetime (days)",
                                data: data.map((data) => data.lifetime),
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
                <h2 className='text-secondary'>Users Lifetime</h2>
                <Line data={this.state.data} options={this.state.options} />
            </div>
        );
    }
}
