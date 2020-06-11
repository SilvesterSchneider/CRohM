import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { FormBuilder, Validators, } from '@angular/forms';
import { IPermissionTemp } from '../mock-roles';
import { BaseDialogInput } from '../../../shared/form/base-dialog-form/base-dialog.component';

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
    @Inject(MAT_DIALOG_DATA) public permissions: IPermissionTemp[],
    private readonly fb: FormBuilder) {
    super(dialogRef, dialog);
  }

  public ngOnInit(): void {
  }
  public onCancle(): void {
    super.confirmDialog();
  }
}
