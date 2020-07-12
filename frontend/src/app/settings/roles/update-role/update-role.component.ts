import { Component, OnInit, Inject } from '@angular/core';
import { IRoleTemp, IPermissionTemp } from '../mock-roles';
import { Validators, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { BaseDialogInput } from '../../../shared/form/base-dialog-form/base-dialog.component';
import { DeleteEntryDialogComponent } from '../../../shared/form/delete-entry-dialog/delete-entry-dialog.component';

@Component({
  selector: 'app-update-role',
  templateUrl: './update-role.component.html',
  styleUrls: ['./update-role.component.scss']
})
export class UpdateRoleDialogComponent extends BaseDialogInput<UpdateRoleDialogComponent> implements OnInit {
  public roleForm = this.fb.group({
    name: ['', Validators.required],
    permissions: ['']
  });
  constructor(
    public dialogRef: MatDialogRef<UpdateRoleDialogComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: IUpdateRoleDialogData,
    private readonly fb: FormBuilder) {
    super(dialogRef, dialog);
  }

  public ngOnInit(): void {
    if (this.data != null && this.data !== undefined) {
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

        this.dialogRef.close({ delete: true });
      }
    });
  }

  private updateForm(): void {
    this.roleForm.patchValue({
      name: this.data.role.name,
      permissions: this.data.role.permissions
    });
  }
}

export interface IUpdateRoleDialogData {
  role: IRoleTemp;
  permissions: IPermissionTemp[];
}
