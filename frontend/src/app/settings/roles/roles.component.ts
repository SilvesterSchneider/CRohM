import { Component, OnInit } from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import { CreateRoleDialogComponent } from './create-role/create-role.component';
import { UpdateRoleDialogComponent } from './update-role/update-role.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { RoleDto, RoleService } from 'src/app/shared/api-generated/api-generated';
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
  permissionGroups: RoleDto[] = new Array<RoleDto>();
  permissions: string[] = new Array<string>();
  public displayedColumns: string[] = ['permission'];
  public dataSource = new MatTableDataSource();

  constructor(public dialog: MatDialog, private permissionService: RoleService) { }

  public ngOnInit(): void {
    this.fillFieldsWithData();
  }

  fillFieldsWithData() {
    this.permissionService.get().subscribe(x => {
        this.permissionGroups = x;
        this.permissionService.getAllClaims(1).subscribe(y => {
          this.permissions = y;
          this.createDynamicColums();
          this.createTableData();
        });
      });
  }

  public openCreateDialog(): void {
    const dialogRef = this.dialog.open(CreateRoleDialogComponent, {
      data: this.permissions,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const permissionsToCreate: string[] = new Array<string>();
        result.permissions.forEach(element => {
          const perm: string = this.permissions.find(a => a === element);
          if (perm != null) {
            permissionsToCreate.push(perm);
          }
        });
        const group: RoleDto = {
          id: 0,
          name: result.name,
          claims: permissionsToCreate
        };
        this.permissionService.post(group).subscribe(x => this.fillFieldsWithData());
      }
    });
  }

  public openUpdateDialog(columnName: string) {
    const dialogRef = this.dialog.open(UpdateRoleDialogComponent, {
      data: {
        role: this.permissionGroups.find(x => x.name === columnName),
        permissions: this.permissions,
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
              this.permissionService.delete(result.id).subscribe(x => this.fillFieldsWithData());
            }
          });
        } else {
          const permissionsToUpdate: string[] = new Array<string>();
          result.permissions.forEach(element => {
            const perm: string = this.permissions.find(a => a === element);
            if (perm != null) {
              permissionsToUpdate.push(perm);
            }
          });
          const groupToUpdate: RoleDto = {
              id: result.id,
              name: result.name,
              claims: permissionsToUpdate
          };
          this.permissionService.put(groupToUpdate).subscribe(x => this.fillFieldsWithData());
        }
      }
    });
  }

  private createTableData() {
    this.tableData = [];
    this.permissions.forEach(perm => {
      const temp: LooseTableObject = {};
      temp.permission = perm;
      this.permissionGroups.forEach(role => {
        temp[role.name] = false;
        if (role.claims.find(a => a === perm)) {
          temp[role.name] = true;
        }
      });
      this.tableData.push(temp);
    });
    this.dataSource.data = this.tableData;
  }

  private createDynamicColums() {
    this.displayedColumns = ['permission'];
    this.permissionGroups.forEach(role => {
      this.displayedColumns.push(role.name);
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}




