import { Component, OnInit } from '@angular/core';
import {ROLES, PERMISSIONS, IRoleTemp} from './mock-roles';

interface LooseTableObject {
  [key: string]: any;
}


@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.scss']
})
export class RolesComponent implements OnInit {

  public tableData: LooseTableObject[] = [];
  public displayedColumns: string[] = ['permission'];


  constructor() { }

  public ngOnInit(): void {
    // load data
    this.createDynamicColums();
    this.createTableData();

  }


  private createTableData() {
    PERMISSIONS.forEach(perm => {
      const temp: LooseTableObject = {};
      temp.permission = perm.name;
      ROLES.forEach(role => {
        temp[role.name] = false;
        if (role.permissions.includes(perm.name)) {
          temp[role.name] = true;
        }
      });
      this.tableData.push(temp);
    });
  }

  private createDynamicColums() {
    ROLES.forEach(role => {
      this.displayedColumns.push(role.name);
    });
  }
}




