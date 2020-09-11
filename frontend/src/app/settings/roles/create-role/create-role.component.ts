import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { FormBuilder, Validators, } from '@angular/forms';
import { BaseDialogInput } from '../../../shared/form/base-dialog-form/base-dialog.component';
import { PermissionDto } from 'src/app/shared/api-generated/api-generated';

@Component({
  selector: 'app-create-role',
  templateUrl: './create-role.component.html',
  styleUrls: ['./create-role.component.scss']
})
export class CreateRoleDialogComponent extends BaseDialogInput<CreateRoleDialogComponent> implements OnInit {
  public roleForm = this.fb.group({
    name: ['', Validators.required],
    permissions: ['']
  });
  constructor(
    public dialogRef: MatDialogRef<CreateRoleDialogComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public permissions: PermissionDto[],
    private readonly fb: FormBuilder) {
    super(dialogRef, dialog);
  }

  public ngOnInit(): void {
  }
  public onCancel(): void {
    super.confirmDialog();
  }

  hasChanged() {
    return !this.roleForm.pristine;
  }
}
