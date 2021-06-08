import React, { Component } from 'react';
import { DataGrid } from '@material-ui/data-grid';
import Noty from 'noty';
export class Grid extends Component {
  static displayName = Grid.name;

  constructor(props) {
    super(props);
    this.state = {
      users: [],
      usersPage: 0,
      columns: [
        { field: 'id', headerName: 'Id', width: 200, editable: false },
        {
          field: 'registrationDate',
          headerName: 'Registration Date',
          type: 'date',
          width: 200,
          editable: true,
        },
        {
          field: 'lastActivityDate',
          headerName: 'LastActivity Date',
          type: 'date',
          width: 200,
          editable: true,
        },
      ],
    };
    this.addRow = this.addRow.bind(this);
    this.onGridRowsUpdated = this.onGridRowsUpdated.bind(this);
    this.getUsers = this.getUsers.bind(this);
  }

  getUsers = () => {
    fetch(`/api/users/GetAllUsers/?pageNumber=${this.state.usersPage}`)
      .then((response) => {
        response.json()
          .then(data => this.setState({
            users: data
          }));

      });
  }

  saveUserData = (user) => {
    if (!this.state.users) return;
    var usersToCreate = this.state.users.filter(item => item.created === true);
    var usersToUpdate = this.state.users.filter(item => item.edited === true && !item.created);
    if ((usersToCreate && usersToCreate.length > 0) || (usersToUpdate && usersToUpdate.length > 0)) {
      usersToCreate.forEach(item => item.id = 0);
      var requestData = usersToCreate.concat(usersToUpdate);
      fetch(`/api/users/CreateOrUpdateUsers`, {
        headers: {
          "Content-Type": "application/json"
        },
        method: "PUT",
        body: JSON.stringify(requestData)

      })
        .then((res) => {

          new Noty({
            text: "Information saved",
            type: "success"
          });
          this.getUsers(this.state.usersPage);
        })
        .catch((res) => new Noty({
          text: res,
          type: "error"
        }).show());
    }


  }

  formattedDate = (d = new Date()) => {
    let month = String(d.getMonth() + 1);
    let day = String(d.getDate());
    const year = String(d.getFullYear());

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return `${day}.${month}.${year}`;
  }

  addRow = () => {
    var ids = this.state.users.map(a => a.id);

    //datagrid can not work without id. need to add max id with pagination will not work, i decided, that id will change by backend;
    function newId() {
      var maxId = Math.max(...ids);
      if (maxId !== -Infinity && !isNaN(parseInt(maxId))) {
        return parseInt(maxId) + 1;
      }
      return 0;
    }

    var newUser = {
      id: newId(),
      registrationDate: this.formattedDate(),
      lastActivityDate: this.formattedDate(),
      created: true
    };

    var joined = this.state.users.concat(
      newUser
    );

    this.setState({ users: joined })


  }


  onGridRowsUpdated = (e) => {
    var rows = this.state.users.slice();
    var user = rows.filter(item => item.id === e.id)[0];
    if (user && e.field && e.props && e.props.value) {
      user[e.field] = this.formattedDate(e.props.value);
      user.edited = true;
    }



    this.setState({ users: rows })
  }

  setPage = (page) => {
    this.setState({ usersPage: page })
  }

  componentDidMount() {
    this.getUsers();
  }

  render() {
    return (
      <div className="d-flex align-items-center justify-content-center mb-4">
        <div className="grid-wrapper">
          <div className="d-flex mb-3">
            <button className="btn btn-primary m-1" onClick={this.addRow}>Add user</button>
            <button className="btn btn-success m-1" onClick={this.saveUserData}>Save</button>
          </div>

          <DataGrid rows={this.state.users} columns={this.state.columns}
            rowsPerPageOptions={[20]}
            paginationMode="server"
            onEditCellChange={this.onGridRowsUpdated}
            page={this.state.usersPage}
            onPageChange={(params) => {
              this.setPage(params.page);
            }} />
        </div>
      </div>
      /*<GridComponent dataSource={this.state.users} allowPaging={true} allowSorting={true}
      pageSettings={this.state.grid.pageSettings} editSettings={this.state.grid.editSettings} 
      toolbar={this.state.grid.toolbar} actionComplete={this.gridActionComplete}>
        <ColumnsDirective>
            <ColumnDirective field="id" headerText="ID" isPrimaryKey={true} width="100"/>
            <ColumnDirective field="userName" headerText="User Name" allowEditing={false} width="100"/>
            <ColumnDirective field="registrationDate" headerText="Registration Date" editType="datepickeredit" type="date" format="dd/MM/yyyy" width="100"/>
            <ColumnDirective field="lastActivityDate" headerText="Last Activity Date" editType="datepickeredit" type="date" format="dd/MM/yyyy" width="100"/>
        </ColumnsDirective>
        <Inject services={[Page, Sort, Edit, Toolbar]} />
      </GridComponent>*/

    );
  }
}