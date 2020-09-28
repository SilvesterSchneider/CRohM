import { BaseDialogInput } from '../../../shared/form/base-dialog-form/base-dialog.component';
import { Component, Inject, OnInit } from '@angular/core';
import { DeleteEntryDialogComponent } from '../../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { RoleDto } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-update-role',
  templateUrl: './update-role.component.html',
  styleUrls: ['./update-role.component.scss']
})
export class UpdateRoleDialogComponent extends BaseDialogInput<UpdateRoleDialogComponent> implements OnInit {
  permissionGroup: RoleDto;
  permissions: string[];
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

  public onCancel(): void {
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
    this.permissionGroup.claims.forEach(a => {
      permStrings.push(a);
    });
    this.roleForm.patchValue({
      id: this.permissionGroup.id,
      name: this.permissionGroup.name,
      permissions: permStrings
    });
  }
}
