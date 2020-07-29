import { Component, OnInit, Inject } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { BaseDialogInput } from '../../../shared/form/base-dialog-form/base-dialog.component';
import { DeleteEntryDialogComponent } from '../../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { PermissionGroupDto, PermissionDto } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-update-role',
  templateUrl: './update-role.component.html',
  styleUrls: ['./update-role.component.scss']
})
export class UpdateRoleDialogComponent extends BaseDialogInput<UpdateRoleDialogComponent> implements OnInit {
  permissionGroup: PermissionGroupDto;
  permissions: PermissionDto[];
  public roleForm = this.fb.group({
    id: [''],
    name: ['', Validators.required],
    permissions: ['']
  });
  constructor(
    public dialogRef: MatDialogRef<UpdateRoleDialogComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private readonly fb: FormBuilder) {
    super(dialogRef, dialog);
  }

  public ngOnInit(): void {
    if (this.data != null && this.data !== undefined) {
      this.permissionGroup = this.data.role;
      this.permissions = this.data.permissions;
      this.updateForm();
    }
  }

  hasChanged() {
    return !this.roleForm.pristine;
  }

  public onCancle(): void {
    super.confirmDialog();
  }

  public onDelete(): void {
    const deleteDialogRef = this.dialog.open(DeleteEntryDialogComponent, {
      data: 'Rolle',
      disableClose: true
    });

    deleteDialogRef.afterClosed().subscribe((deleteResult) => {
      if (deleteResult.delete) {
        this.dialogRef.close({ delete: true, id: this.permissionGroup.id });
      }
    });
  }

  private updateForm(): void {
    const permStrings: string[] = new Array<string>();
    this.permissionGroup.permissions.forEach(a => {
      permStrings.push(a.name);
    });
    this.roleForm.patchValue({
      id: this.permissionGroup.id,
      name: this.permissionGroup.name,
      permissions: permStrings
    });
  }
}

