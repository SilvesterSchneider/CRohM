import { Component, OnInit } from '@angular/core';
import {ROLES, PERMISSIONS, IRoleTemp, IPermissionTemp} from './mock-roles';
import {MatDialog} from '@angular/material/dialog';
import { CreateRoleDialogComponent } from './create-role/create-role.component';
import { UpdateRoleDialogComponent } from './update-role/update-role.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { MatTableDataSource } from '@angular/material/table';

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
  public dataSource = new MatTableDataSource();


  constructor(public dialog: MatDialog) { }

  public ngOnInit(): void {
    // TODO:
    // load data
    this.createDynamicColums();
    this.createTableData();
    this.dataSource.data = this.tableData;

  }

  public openCreateDialog(): void {
    const dialogRef = this.dialog.open(CreateRoleDialogComponent, {
      data: PERMISSIONS,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // TODO:
        // call backend to add role
        // call backend for all roles
        console.log(result);
      }
    });
  }

  public openUpdateDialog(columnName: string) {
    const dialogRef = this.dialog.open(UpdateRoleDialogComponent, {
      data: {
        role: ROLES.find(x => x.name === columnName),
        permissions: PERMISSIONS,
        disableClose: true
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.delete) {
          const deleteDialogRef = this.dialog.open(DeleteEntryDialogComponent, {
            data: columnName,
            disableClose: true
          });

          deleteDialogRef.afterClosed().subscribe(deleteResult => {
            if (deleteResult.delete) {
              // TODO:
              // call backend to delete role
            }
          });
        } else {
          // TODO:
          // call backend to update role
          // call backend for all roles
        }
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

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}




