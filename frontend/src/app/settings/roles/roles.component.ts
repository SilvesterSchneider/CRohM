import { Component, OnInit } from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import { CreateRoleDialogComponent } from './create-role/create-role.component';
import { UpdateRoleDialogComponent } from './update-role/update-role.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { PermissionGroupDto, PermissionDto, PermissionsService } from 'src/app/shared/api-generated/api-generated';

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
  permissionGroups: PermissionGroupDto[] = new Array<PermissionGroupDto>();
  permissions: PermissionDto[] = new Array<PermissionDto>();
  public displayedColumns: string[] = ['permission'];

  constructor(public dialog: MatDialog, private permissionService: PermissionsService) { }

  public ngOnInit(): void {
    this.fillFieldsWithData();
  }

  fillFieldsWithData() {
    this.permissionService.getAllPermissionGroups().subscribe(x => {
        this.permissionGroups = x;
        this.permissions = new Array<PermissionDto>();
        this.permissionGroups.forEach(y => {
            y.permissions.forEach(z => {
                if (!this.permissions.find(a => a.name === z.name)) {
                  this.permissions.push(z);
                }
              });
          });
        this.createDynamicColums();
        this.createTableData();
      });
  }

  public openCreateDialog(): void {
    const dialogRef = this.dialog.open(CreateRoleDialogComponent, {
      data: this.permissions,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const permissionsToCreate: PermissionDto[] = new Array<PermissionDto>();
        result.permissions.forEach(element => {
          const perm: PermissionDto = this.permissions.find(a => a.name === element);
          if (perm != null) {
            permissionsToCreate.push(perm);
          }
        });
        const group: PermissionGroupDto = {
          id: 0,
          name: result.name,
          permissions: permissionsToCreate
        };
        this.permissionService.createPermissionGroup(group).subscribe(x => this.fillFieldsWithData());
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
              this.permissionService.deletePermissionGroup(result.id).subscribe(x => this.fillFieldsWithData());
            }
          });
        } else {
          const permissionsToUpdate: PermissionDto[] = new Array<PermissionDto>();
          result.permissions.forEach(element => {
            const perm: PermissionDto = this.permissions.find(a => a.name === element);
            if (perm != null) {
              permissionsToUpdate.push(perm);
            }
          });
          const groupToUpdate: PermissionGroupDto = {
              id: result.id,
              name: result.name,
              permissions: permissionsToUpdate
          };
          this.permissionService.updatePermissionGroup(groupToUpdate).subscribe(x => this.fillFieldsWithData());
        }
      }
    });
  }

  private createTableData() {
    this.tableData = [];
    this.permissions.forEach(perm => {
      const temp: LooseTableObject = {};
      temp.permission = perm.name;
      this.permissionGroups.forEach(role => {
        temp[role.name] = false;
        if (role.permissions.find(a => a.name === perm.name)) {
          temp[role.name] = true;
        }
      });
      this.tableData.push(temp);
    });
  }

  private createDynamicColums() {
    this.displayedColumns = ['permission'];
    this.permissionGroups.forEach(role => {
      this.displayedColumns.push(role.name);
    });
  }
}




