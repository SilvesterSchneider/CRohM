import { Component, OnInit } from '@angular/core';
import {ROLES, PERMISSIONS, IRoleTemp, IPermissionTemp} from './mock-roles';
import {MatDialog} from '@angular/material/dialog';
import { CreateRoleDialogComponent } from './create-role/create-role.component';

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


  constructor(public dialog: MatDialog) { }

  public ngOnInit(): void {
    // load data
    this.createDynamicColums();
    this.createTableData();

  }

  public openDialog(): void {
    // const dialogRef = this.dialog.open<CreateRoleDialogComponent, IPermissionTemp[], IRoleTemp>(CreateRoleDialogComponent, {
    //   data: PERMISSIONS
    // });

    const dialogRef = this.dialog.open(CreateRoleDialogComponent, {
      data: PERMISSIONS
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
      // add automatic to table
      // call backend to add role
        console.log(result);
      }
    });
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




